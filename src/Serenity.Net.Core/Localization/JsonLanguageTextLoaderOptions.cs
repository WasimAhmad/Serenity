namespace Serenity.Localization;

/// <summary>
/// Options for JsonLanguageTextLoader.
/// </summary>
public class JsonLanguageTextLoaderOptions
{
    /// <summary>
    /// Gets the list of language sets to be scanned for JSON text files.
    /// </summary>
    public List<LanguageSet> LanguageSets { get; } = new();
}
