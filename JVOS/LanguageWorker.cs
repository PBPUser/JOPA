using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JVOS.LanguageWorker;

namespace JVOS
{
    public static class LanguageWorker
    {
        private static Language? Current;
        public static List<Language> Languages = new List<Language>();
        private static string LanguagesLoc;

        static LanguageWorker()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                LanguagesLoc = "languages.jv";
            else
                LanguagesLoc = $"{AppContext.BaseDirectory}\\languages.jv";
            Load();
        }

        public static void Test()
        {

        }

        public static void SetLanguage(Language language)
        {
            Current = language;
            App.Current.Resources["LangShortName"] = language.ShortName;
            Save();
        }

        private static void Load()
        {
            if(!File.Exists(LanguagesLoc))
            {
                LoadDefaults();
                return;
            }
            Languages.Clear();
            using (var streamReader = File.OpenText(LanguagesLoc))
            {
                string[] strj = streamReader.ReadLine().Split("=");
                if (strj.Length == 3)
                    Current = Create(strj[1], strj[0], strj[2].Replace("$%newLine%$", "\n").Replace("$$", "$").Replace("%%", "%"));
                App.Current.Resources["LangShortName"] = Current.Value.ShortName;
                while (!streamReader.EndOfStream)
                {
                    string[] strs = streamReader.ReadLine().Split("=");
                    if(strs.Length == 3)
                    {
                        if (Languages.Where(x => x.ShortName == strs[1]).Count() > 0)
                            continue;
                        Languages.Add(Create(strs[1], strs[0], strs[2].Replace("$%newLine%$", "\n").Replace("$$", "$").Replace("%%", "%")));
                    }
                }
            }
            if (Languages.Count == 0)
                LoadDefaults();
            else
                Save();
        }

        private static void LoadDefaults()
        {
            Languages.Clear();
            Current = Create("Akobian", "AKOB", "Default JVOS input language");
            App.Current.Resources["LangShortName"] = Current.Value.ShortName;
            Languages.Add(Current.Value);
            Languages.Add(Create("Joyousmicorian", "JV", "Another Default JVOS input language"));
            Save();
        }

        private static void Save()
        {
            using (var streamWriter = File.CreateText(LanguagesLoc))
            {
                streamWriter.WriteLine($"{Current.Value.ShortName}={Current.Value.Name}={Current.Value.Description.Replace("$", "$$").Replace("%", "%%").Replace("\n", "$%newLine%$")}");
                foreach (var language in Languages) {
                    streamWriter.WriteLine($"{language.ShortName}={language.Name}={language.Description.Replace("$", "$$").Replace("%", "%%").Replace("\n", "$%newLine%$")}");
                }
            }
        }

        public static void AddLanguage(Language lang)
        {
            Languages.Add(lang);
            Save();
        }

        public static void RemoveLanguage(Language lang) {
            Languages.Remove(lang);
            Save();
        }

        public static Language Create(string name, string shortname, string desc)
        {
            return new Language
            {
                Name = name,
                ShortName = shortname,
                Description = desc
            };
        }
    }

    public struct Language
    {
        public string Name;
        public string ShortName;
        public string Description;
    }
}
