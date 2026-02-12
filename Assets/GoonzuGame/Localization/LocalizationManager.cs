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

        public string Translate(string key)
        {
            if (LanguageFiles.ContainsKey(CurrentLanguage) && LanguageFiles[CurrentLanguage].ContainsKey(key))
                return LanguageFiles[CurrentLanguage][key];
            return key;
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
