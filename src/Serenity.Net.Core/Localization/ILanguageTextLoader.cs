namespace Serenity.Localization;

/// <summary>
/// Abstraction for a service that can load all texts for a specific language.
/// </summary>
public interface ILanguageTextLoader
{
    /// <summary>
    /// Loads all texts for the specified language ID.
    /// </summary>
    /// <param name="languageId">The language ID (e.g., "en", "de-DE").</param>
    /// <returns>A dictionary containing all text keys and their translations
    /// for the specified language. Returns an empty dictionary if the language
    /// is not found or has no texts.</returns>
    IDictionary<string, string> LoadTexts(string languageId);
}
