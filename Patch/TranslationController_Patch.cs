using System;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

//using TranslationController = GIGNEFLFPDE;

namespace BetterLanguages.Patch
{
	class TranslationController_Patch
	{
		[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.Awake))]
		class TranslationController_Awake_Patch
		{
			public static void Postfix()
			{
				LanguageManager.LoadLanguages();
			}
		}
		/// <summary>
		/// Replacement of all the GetString functions ensuring that translations applies anywhere where it is possible.
		/// </summary>
		[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
		class TranslationController_GetString_StringNames_Patch
		{
			public static bool Prefix(StringNames HKOIECMDOKL, ref string __result, Il2CppReferenceArray<Il2CppSystem.Object> EBKIKEILMLF)
			{
				__result = LanguageManager.GetString(HKOIECMDOKL, EBKIKEILMLF);
				return false;
			}
		}

		/// <summary>
		/// Replacement of all the GetString functions ensuring that translations applies anywhere where it is possible.
		/// </summary>
		[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(SystemTypes))]
		class TranslationController_GetString_SystemTypes_Patch
		{
			public static bool Prefix(SystemTypes BNHNBDIBABN, ref string __result)
			{
				__result = LanguageManager.GetString(TranslationController.SystemTypesToStringNames[(int)BNHNBDIBABN]);
				return false;
			}
		}

		/// <summary>
		/// Replacement of all the GetString functions ensuring that translations applies anywhere where it is possible.
		/// </summary>
		[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(TaskTypes))]
		class TranslationController_GetString_TaskTypes_Patch
		{
			public static bool Prefix(TaskTypes OJGILIGMHKL, ref string __result)
			{
				__result = LanguageManager.GetString(TranslationController.TaskTypesToStringNames[(int)((byte)OJGILIGMHKL)]);
				return false;
			}
		}

		/// <summary>
		/// Replace that function so that the 
		/// </summary>
		[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.SetLanguage))]
		class TranslationController_SetLanguage_Patch
		{
			public static bool Prefix(TranslationController __instance, TextAsset PAPBPGICHCK)
			{
				string name = PAPBPGICHCK.name;
				BetterLanguagesPlugin.log.LogInfo("Set language to " + name);
				LanguageManager.SetLanguage(name);

				// Setting the SaveManager's last language to a one that would work in any cases.
				SaveManager.LastLanguage = 0;
				__instance.CurrentLanguage = new LanguageUnit(__instance.Languages[0], __instance.Images[0].Images);

				for (int i = 0; i < __instance.ActiveTexts.Count; i++)
				{
					__instance.ActiveTexts[i].ResetText();
				}
				return false;
			}
		}
	}
}
