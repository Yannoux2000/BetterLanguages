using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;

namespace BetterLanguages
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    public class BetterLanguagesPlugin : BasePlugin
    {
        public const string Id = "com.inxs212.betterlanguages";

        public ConfigEntry<string> LanguagesPath { get; private set; }
        public ConfigEntry<string> FallBackLanguage { get; private set; }
        public Harmony Harmony { get; } = new Harmony(Id);

        public static ManualLogSource log;

        public override void Load()
        {
            log = base.Log;
            LanguagesPath = Config.Bind("Paths", "Localization Directory", "Localizations");
            FallBackLanguage = Config.Bind("Language", "Default Language", "English");


            Harmony.PatchAll();
        }

        internal static string LocDir()
        {
            return PluginSingleton<BetterLanguagesPlugin>.Instance.LanguagesPath.Value;
        }
    }
}
