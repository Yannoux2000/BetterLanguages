using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using BetterLanguages.Data;

namespace BetterLanguages.Patch
{
    class LanguageSetter_Patch
	{
		public static TextAsset[] GenerateAssets()
        {
			List<TextAsset> texts = new List<TextAsset>();
			TextAsset source = DestroyableSingleton<TranslationController>.Instance.Languages[0];

			foreach (KeyValuePair<string, LiteLanguageUnit> lang in LanguageManager.AllLanguages)
            {
				TextAsset copy = Object.Instantiate<TextAsset>(source);
				copy.name = lang.Value.DisplayName;
				texts.Add(copy);
            }
			BetterLanguagesPlugin.log.LogInfo("Set as selectable " + texts.Count + " Languages.");
			return texts.ToArray();
        }

		[HarmonyPatch(typeof(LanguageSetter), nameof(LanguageSetter.Start))]
		class LanguageSetter_Start_Patch
		{
			public static bool Prefix(LanguageSetter __instance)
			{
				//only line that is supposed to differ from origin
				TextAsset[] languages = GenerateAssets();

				Vector3 vector = new Vector3(0f, __instance.ButtonStart, -1f);
				__instance.AllButtons = new LanguageButton[languages.Length];
				for (int i = 0; i < languages.Length; i++)
				{
					LanguageButton button = UnityEngine.Object.Instantiate<LanguageButton>(__instance.ButtonPrefab, __instance.ButtonParent.Inner);
					__instance.AllButtons[i] = button;
					button.Language = languages[i];
					button.Title.Text = languages[i].name;
					if ((long)i == (long)((ulong)SaveManager.LastLanguage))
					{
						button.Title.Color = Color.green;
					}

                    System.Action p = delegate ()
                        {
                            __instance.SetLanguage(button);
                        };

                    button.Button.OnClick.AddListener(p);
					button.transform.localPosition = vector;
					vector.y -= __instance.ButtonHeight;
				}
				__instance.ButtonParent.YBounds.max = Mathf.Max(0f, -vector.y - __instance.ButtonStart * 2f);
				return false;
			}
		}
	}
}
