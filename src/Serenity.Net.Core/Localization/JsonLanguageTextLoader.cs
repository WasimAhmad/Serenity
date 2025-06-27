using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Text.Json; // Prefer System.Text.Json if possible for Serenity's direction

namespace Serenity.Localization;

/// <summary>
/// Configuration for a set of language files to be loaded.
/// </summary>
/// <param name="Provider">The file provider.</param>
/// <param name="SubPath">The subpath to scan for JSON files.</param>
/// <param name="Recursive">Whether to scan recursively (default true).</param>
public record LanguageSet(IFileProvider Provider, string SubPath, bool Recursive = true);

/// <summary>
/// Loads language texts from JSON files based on configured language sets.
/// </summary>
using Microsoft.Extensions.Options;

public class JsonLanguageTextLoader : ILanguageTextLoader
{
    private readonly JsonLanguageTextLoaderOptions options;

    /// <summary>
    /// Creates a new instance of <see cref="JsonLanguageTextLoader"/>.
    /// </summary>
    /// <param name="options">The options containing language sets to scan.</param>
    /// <exception cref="ArgumentNullException">options is null.</exception>
    public JsonLanguageTextLoader(IOptions<JsonLanguageTextLoaderOptions> options)
    {
        this.options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
    }

    /// <inheritdoc/>
    public IDictionary<string, string> LoadTexts(string languageId)
    {
        ArgumentException.ThrowIfNullOrEmpty(languageId);

        var combinedTexts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (options.LanguageSets == null) // Should not happen if options are properly configured
            return combinedTexts;

        foreach (var set in options.LanguageSets)
        {
            LoadFromSet(set.Provider, set.SubPath, languageId, combinedTexts, set.Recursive);
        }

        return combinedTexts;
    }

    private void LoadFromSet(IFileProvider provider, string subpath, string targetLanguageId,
        Dictionary<string, string> targetDictionary, bool recursive)
    {
        var contents = provider.GetDirectoryContents(subpath);
        if (contents is null || !contents.Exists)
            return;

        // Order files to ensure deterministic loading if keys overlap (last one wins)
        foreach (var entry in contents.OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase))
        {
            if (entry.IsDirectory)
            {
                if (recursive)
                    LoadFromSet(provider, Path.Combine(subpath, entry.Name), targetLanguageId, targetDictionary, recursive);
                continue;
            }

            if (!entry.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                continue;

            var fileLanguageId = JsonLocalTextRegistration.ParseLanguageIdFromPath(entry.Name);
            if (fileLanguageId != targetLanguageId)
                continue;

            string? jsonContent;
            try
            {
                using var stream = entry.CreateReadStream();
                using var sr = new StreamReader(stream);
                jsonContent = sr.ReadToEnd().TrimToNull();
            }
            catch (Exception ex)
            {
                // Log or handle error reading file if necessary
                System.Diagnostics.Debug.WriteLine($"Error reading language file {entry.Name}: {ex.Message}");
                continue;
            }

            if (jsonContent is null)
                continue;

            try
            {
                // Using System.Text.Json for parsing. Serenity.JSON.Parse might use Newtonsoft by default.
                // This might need adjustment based on Serenity's standard JSON handling.
                // For now, assuming Dictionary<string, object> is compatible with ProcessNestedDictionary.
                var nestedTexts = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (nestedTexts is not null)
                {
                    // Re-use the existing logic from JsonLocalTextRegistration to flatten the dictionary.
                    // This helper method might need to be made public or refactored if it's internal.
                    // For now, assume it's accessible or we'll inline/adapt its logic.
                    JsonLocalTextRegistration.ProcessNestedDictionary(nestedTexts, "", targetDictionary);
                }
            }
            catch (JsonException ex)
            {
                // Log or handle JSON parsing error if necessary
                System.Diagnostics.Debug.WriteLine($"Error parsing language file {entry.Name}: {ex.Message}");
                continue;
            }
        }
    }
}
