using System.Collections.Generic;
using System.Globalization;

namespace GameEngine.Localization
{
    /// <summary>
    /// Manages localization, language selection, and translation lookup.
    /// </summary>
    public class LocalizationManager
    {
        private Dictionary<string, Dictionary<string, string>> _translations = new();
        public string CurrentLanguage { get; private set; } = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        public void LoadLanguage(string languageCode)
        {
            // TODO: Load language file from disk (e.g., JSON, CSV)
            CurrentLanguage = languageCode;
        }

        public string Translate(string key)
        {
            if (_translations.TryGetValue(CurrentLanguage, out var dict) && dict.TryGetValue(key, out var value))
                return value;
            return key;
        }

        public void AddTranslation(string languageCode, string key, string value)
        {
            if (!_translations.ContainsKey(languageCode))
                _translations[languageCode] = new Dictionary<string, string>();
            _translations[languageCode][key] = value;
        }
    }
}
