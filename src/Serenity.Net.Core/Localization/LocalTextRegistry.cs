using Microsoft.Extensions.DependencyInjection; // Required for IServiceProvider

namespace Serenity.Localization;

/// <summary>
/// Default ILocalTextRegistry implementation.
/// </summary>
/// <seealso cref="ILocalTextRegistry" />
/// <seealso cref="IRemoveAll" />
/// <remarks>
/// This implementation also supports a "pending approval" mode. If your site needs some moderator
/// approval before translations are published, you may put your site to this mode when
/// some moderator is using the site by registering an ILocalTextContext provider. Thus,
/// moderators can see unapproved texts while they are logged in to the site.
/// </remarks>
public class LocalTextRegistry : ILocalTextRegistry, IRemoveAll, IGetAllTexts, ILanguageFallbacks
{
    private readonly ConcurrentDictionary<LanguageIdKeyPair, string?> approvedTexts = new();
    private readonly ConcurrentDictionary<LanguageIdKeyPair, string?> pendingTexts = new();
    private readonly ConcurrentDictionary<string, string> languageFallbacks = new(StringComparer.OrdinalIgnoreCase);
    private readonly IServiceProvider serviceProvider; // To resolve ILanguageTextLoader

    // Tracks languages for which loading has been attempted/completed.
    // Lazy<object> ensures the loader func is called once per language.
    // Using object as T for Lazy as we only care about the execution of the factory.
    private readonly ConcurrentDictionary<string, Lazy<object>> loadedLanguageFlags = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalTextRegistry"/> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider to resolve <see cref="ILanguageTextLoader"/>.</param>
    /// <exception cref="ArgumentNullException">serviceProvider is null</exception>
    public LocalTextRegistry(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Adds a local text entry to the registry
    /// </summary>
    /// <param name="languageID">Language ID (e.g. en-US, tr-TR)</param>
    /// <param name="key">Local text key</param>
    /// <param name="text">Translated text</param>
    public void Add(string languageID, string key, string? text)
    {
        if (languageID == null)
            throw new ArgumentNullException(nameof(languageID));

        if (key == null)
            throw new ArgumentNullException(nameof(languageID));

        if (text is null)
        {
            // probably null entry in texts.xy.json, save it to remember the key only
            var pair = new LanguageIdKeyPair(LocalText.InvariantLanguageID, key);
            approvedTexts.GetOrAdd(pair, (string?)null);
        }
        else
            approvedTexts[new LanguageIdKeyPair(languageID, key)] = text;
    }

    /// <summary>
    /// Adds a pending approval local text entry to the registry. These texts can only be seen
    /// while moderators are browsing the site. You can determine which users are moderators by
    /// implementing ILocalTextContext interface, and registering it through the service locator.
    /// </summary>
    /// <param name="languageID">Language ID (e.g. en-US, tr-TR)</param>
    /// <param name="key">Local text key</param>
    /// <param name="text">Translated text</param>
    public void AddPending(string languageID, string key, string text)
    {
        if (languageID == null)
            throw new ArgumentNullException(nameof(languageID));

        if (key == null)
            throw new ArgumentNullException(nameof(key));

        pendingTexts[new LanguageIdKeyPair(languageID, key)] = text;
    }

    /// <summary>
    /// Converts the local text key to its representation in requested language. Looks up text
    /// in requested language, its Fallbacks and invariant language in order. If not found in any,
    /// null is returned. See SetLanguageFallback for information about language fallbacks.
    /// </summary>
    /// <param name="languageID">Language ID.</param>
    /// <param name="textKey">Local text key</param>
    /// <param name="pending">If pending approval texts to be used, true.</param>
    public string? TryGet(string languageID, string textKey, bool pending)
    {
        if (languageID == null)
            throw new ArgumentNullException(nameof(languageID));

        if (textKey == null)
            throw new ArgumentNullException(nameof(textKey));

        // Ensure the requested language is loaded (or loading is attempted)
        EnsureLanguageLoaded(languageID);

        var originalLanguageID = languageID;
        var circularCheck = 0;
        LanguageIdKeyPair k;

        do
        {
            // Ensure fallback language is also loaded if different
            if (languageID != originalLanguageID && languageID != LocalText.InvariantLanguageID)
                EnsureLanguageLoaded(languageID);

            k = new LanguageIdKeyPair(languageID, textKey);

            if (pending && pendingTexts.TryGetValue(k, out string? s))
            {
                if (s != null)
                    return s;
            }
            else if (approvedTexts.TryGetValue(k, out s))
                return s;
            
            if (languageID == LocalText.InvariantLanguageID)
                return null; // Already checked invariant, nothing more to do.

            languageID = TryGetLanguageFallback(languageID) ?? LocalText.InvariantLanguageID;
        }
        while (circularCheck++ < 10); // Max 10 fallback attempts to prevent infinite loops

        return null;
    }

    private void EnsureLanguageLoaded(string languageID)
    {
        if (languageID == null) // Should not happen if called from TryGet with validated languageID
            return;

        // For invariant language, assume it's implicitly "loaded" or doesn't need explicit file loading
        // unless specific invariant files are part of the ILanguageTextLoader's scope.
        // If ILanguageTextLoader can return texts for "invariant", it will be handled.
        if (languageID == LocalText.InvariantLanguageID &&
            !serviceProvider.GetRequiredService<ILanguageTextLoader>().LoadTexts(LocalText.InvariantLanguageID).Any())
        {
            // If invariant loader provides no texts, mark it as loaded to avoid re-check.
            // Or, if invariant texts are special-cased and not file-based, this ensures we don't try to load them.
             loadedLanguageFlags.GetOrAdd(languageID, new Lazy<object>(() => true, LazyThreadSafetyMode.ExecutionAndPublication));
             return;
        }

        _ = loadedLanguageFlags.GetOrAdd(languageID, langId => new Lazy<object>(() =>
        {
            // This factory func is executed only once per languageID due to Lazy<T> behavior.
            try
            {
                var loader = serviceProvider.GetRequiredService<ILanguageTextLoader>();
                var texts = loader.LoadTexts(langId);
                if (texts != null)
                {
                    foreach (var pair in texts)
                    {
                        // Assuming Add handles null values for text appropriately (e.g. for key tracking)
                        // The current ILanguageTextLoader returns IDictionary<string, string>, so values shouldn't be null.
                        Add(langId, pair.Key, pair.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using ILogger from serviceProvider if available)
                // Or rethrow if critical, or handle as appropriate for the application.
                // For now, if loading fails, the language will appear to have no texts.
                // Consider how to handle retries or permanent failure states.
                System.Diagnostics.Debug.WriteLine($"Error loading texts for language {langId}: {ex.Message}");
            }
            return true; // Dummy object, indicates loading was attempted.
        }, LazyThreadSafetyMode.ExecutionAndPublication)).Value; // .Value ensures the lazy factory is executed if not already
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetLanguageFallbacks(string languageID)
    {
        if (string.IsNullOrEmpty(languageID))
            yield break;

        var circularCheck = 0;
        do
        {
            if (languageID == LocalText.InvariantLanguageID)
                yield break;

            yield return languageID = TryGetLanguageFallback(languageID) ?? LocalText.InvariantLanguageID;
        }
        while (circularCheck++ < 10);
    }

    /// <inheritdoc/>
    public void SetLanguageFallback(string languageID, string fallbackID)
    {
        if (languageID == null)
            throw new ArgumentNullException(nameof(languageID));

        languageFallbacks[languageID] = fallbackID ?? throw new ArgumentNullException(nameof(fallbackID));
    }

    private string? TryGetLanguageFallback(string languageID)
    {
        if (string.IsNullOrEmpty(languageID))
            return null;

        return languageFallbacks.GetOrAdd(languageID, static id =>
        {
            if (id.Length == 5 &&
                id[2] == '-' &&
                id[0] >= 'a' && id[0] <= 'z' &&
                id[1] >= 'a' && id[1] <= 'z' &&
                id[3] >= 'A' && id[3] <= 'Z' &&
                id[4] >= 'A' && id[4] <= 'Z')
            {
                return id[..2];
            }

            return LocalText.InvariantLanguageID;
        });
    }

    /// <summary>
    ///   Gets all available text keys (that has a translation in language or any of its
    ///   language fallbacks) and their local texts.</summary>
    /// <param name="languageID">
    ///   Language ID (required).</param>
    /// <param name="pending">
    ///   True if pending texts should be returned (e.g. in preview/edit mode).</param>
    /// <returns>
    ///   A dictionary of all texts in the language.</returns>
    public Dictionary<string, string> GetAllAvailableTextsInLanguage(string languageID, bool pending)
    {
        if (languageID == null)
            throw new ArgumentNullException(nameof(languageID));

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Temporary list to hold language IDs in fallback order
        var languageHierarchy = new List<string>();
        string currentLang = languageID;
        int circularCheck = 0;

        // 1. Ensure all relevant languages are loaded and build the hierarchy
        while (circularCheck++ < 10) // Max 10 fallback attempts
        {
            EnsureLanguageLoaded(currentLang);
            languageHierarchy.Add(currentLang);
            if (currentLang == LocalText.InvariantLanguageID)
                break;
            currentLang = TryGetLanguageFallback(currentLang) ?? LocalText.InvariantLanguageID;
        }

        // 2. Iterate through the hierarchy from most specific to least specific (invariant)
        //    and populate the result dictionary.
        foreach (var langToScan in languageHierarchy)
        {
            // Handle pending texts first if requested
            if (pending)
            {
                foreach (var pair in pendingTexts)
                {
                    if (pair.Key.LanguageId == langToScan &&
                        pair.Value != null && // Ensure there's a translation
                        !result.ContainsKey(pair.Key.Key)) // Add only if key not already added from a more specific lang
                    {
                        result[pair.Key.Key] = pair.Value;
                    }
                }
            }

            // Handle approved texts
            foreach (var pair in approvedTexts)
            {
                if (pair.Key.LanguageId == langToScan &&
                    pair.Value != null && // Ensure there's a translation (or a non-null marker for a key)
                    !result.ContainsKey(pair.Key.Key))  // Add only if key not already added
                {
                    result[pair.Key.Key] = pair.Value;
                }
                else if (pair.Key.LanguageId == langToScan &&
                         pair.Value == null && // Key exists but has null text (marker for key existence)
                         !result.ContainsKey(pair.Key.Key))
                {
                    // If a key is explicitly registered with null (e.g. from a JSON file with "key": null),
                    // and we want to acknowledge its existence without a translation,
                    // this behavior might need adjustment. For now, only non-null values are added.
                    // If the goal is to get *any* text, including explicitly null ones, this logic changes.
                    // The current TryGet would return null for such a key.
                    // Let's assume GetAllAvailableTextsInLanguage should only return actual translated strings.
                }
            }
        }
        return result;
    }

    /// <inheritdoc/>
    public IDictionary<LanguageIdKeyPair, string?> GetAllTexts(bool pending)
    {
        return pending ? pendingTexts : approvedTexts;
    }

    /// <summary>
    /// Gets all text keys that is currently registered in any language
    /// </summary>
    public HashSet<string> GetAllTextKeys(bool pending)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var k in (pending ? pendingTexts : approvedTexts).Keys)
            result.Add(k.Key);
        return result;
    }

    /// <summary>
    /// Removes all cached texts
    /// </summary>
    public void RemoveAll()
    {
        approvedTexts.Clear();
        pendingTexts.Clear();
        languageFallbacks.Clear();
        loadedLanguageFlags.Clear(); // Also clear loaded language flags
    }
}