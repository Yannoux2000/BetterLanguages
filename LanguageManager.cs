using BetterLanguages.Data;
using Reactor;
using System.Collections.Generic;
using System.Globalization;
using UnhollowerBaseLib;

namespace BetterLanguages
{
    public class LanguageManager
    {
		private static List<LanguageManager> managers = new List<LanguageManager>();

		public static string MissingString { get; } = "STRMISS";

		//public static List<string> FullCoverage { get; private set; } = new List<string>();

		/// <summary>
		/// List of all languages that has been found on last load
		/// </summary>
		public static Dictionary<string, LiteLanguageUnit> AllLanguages { get; private set; } = new Dictionary<string, LiteLanguageUnit>();

		private static FullLanguageUnit FallbackLanguage;
		private static FullLanguageUnit CurrentLanguage;

		public static string CurrentLanguageName { get; private set; }

		private static LanguageLoader loader = new LanguageLoader();

/*		public LanguageManager() {
			managers.Add(this);
		}*/

/*		public static LanguageManager Start()
		{
			LanguageManager Instance = new LanguageManager();
			Instance.ReloadLanguages();
			return Instance;
		}*/

		/// <summary>
		/// In case you See issues, you may call this function so that the data be reset
		/// </summary>
		public static void ReloadLanguages()
		{
			loader.LoadLanguages();
			AllLanguages = loader.getLanguagesList();

			LiteLanguageUnit lite = null;
			string FallbackKey = PluginSingleton<BetterLanguagesPlugin>.Instance.FallBackLanguage.Value;
			CultureInfo ci = LanguageLoader.GetCultureInfo(FallbackKey);
			if (ci == null || !AllLanguages.TryGetValue(ci.NativeName, out lite))
            {
				AllLanguages.TryGetValue(FallbackKey, out lite);
			}

			FallbackLanguage = loader.GetLang(lite);
			if (CurrentLanguage == null) CurrentLanguage = FallbackLanguage;
		}

		/// <summary>
		/// This function is called inside the TranslationController Awake Patch
		/// </summary>
		public static void LoadLanguages()
		{
            if (!loader.Loaded)
            {
				ReloadLanguages();
			}
        }

		/// <summary>
		/// Sets language corresponding to the same title.
		/// </summary>
		/// <param name="langName">language name to be set</param>
		public static void SetLanguage(string langName)
        {
			LiteLanguageUnit lite = null;
			if(AllLanguages.TryGetValue(langName, out lite))
			{
				BetterLanguagesPlugin.log.LogMessage("Putain ça marche ou quoi ?");
				CurrentLanguage = loader.GetLang(lite);
				CurrentLanguageName = langName;
			}
        }

		public static string GetString(StringNames stringId, Il2CppReferenceArray<Il2CppSystem.Object> parts)
		{
			string ret;
			if(CurrentLanguage.GetString(out ret, stringId, parts))
            {
				return ret;
            }
			if(FallbackLanguage.GetString(out ret, stringId, parts))
            {
				return ret;
            }
			return MissingString;
		}
		public static string GetString(StringNames stringId)
		{
			string ret;
			if (CurrentLanguage.GetString(out ret, stringId))
			{
				return ret;
			}
			if (FallbackLanguage.GetString(out ret, stringId))
			{
				return ret;
			}
			return MissingString;
		}

		/// <summary>
		/// Get the translated string for the registered key in the Language Units currently in use. also handles formatting.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="parts">Parameters needed for formating</param>
		/// <returns></returns>
		public static string GetString(string key, params object[] parts)
		{
			string ret;
			if (CurrentLanguage.GetString(out ret, key, parts))
			{
				return ret;
			}
			if (FallbackLanguage.GetString(out ret, key, parts))
			{
				return ret;
			}
			return MissingString;
		}
	}
}
