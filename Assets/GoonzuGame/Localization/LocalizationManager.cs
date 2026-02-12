using System;

namespace GoonzuGame.Localization
{
    using System.Collections.Generic;

    public class LocalizationManager
    {
        public string CurrentLanguage { get; set; }
        public Dictionary<string, Dictionary<string, string>> LanguageFiles { get; set; }

        public LocalizationManager()
        {
            CurrentLanguage = "en";
            LanguageFiles = new Dictionary<string, Dictionary<string, string>>();
        }

        public void SetLanguage(string language)
        {
            CurrentLanguage = language;
            Console.WriteLine($"Language set to: {language}");
        }

            private Dictionary<string, string> translations = new Dictionary<string, string>();
            public void AddTranslation(string key, string value) {
                translations[key] = value;
            }
            public new string Translate(string key) {
                return translations.ContainsKey(key) ? translations[key] : base.Translate(key);
            }

        public void LoadLanguageFiles()
        {
            // Example: load English and Spanish
            LanguageFiles["en"] = new Dictionary<string, string> { { "hello", "Hello" }, { "bye", "Goodbye" } };
            LanguageFiles["es"] = new Dictionary<string, string> { { "hello", "Hola" }, { "bye", "Adiós" } };
            Console.WriteLine("Language files loaded.");
        }
    }
}
