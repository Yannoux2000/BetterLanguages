using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using BetterLanguages.Data;

namespace BetterLanguages
{
    internal class LanguageLoader
    {
        public bool Created { get; private set; }
        public bool Loaded { get; private set; }

        public string Dir { get; private set; } = "";
        public Dictionary<string, LiteLanguageUnit> AllLanguages { get; set; } = new Dictionary<string, LiteLanguageUnit>();
        private string FullPath { get; } = Path.Combine(Directory.GetCurrentDirectory(), BetterLanguagesPlugin.LocDir());

        public LanguageLoader(string dir = "")
        {
            Dir = dir;
            FullPath = Path.Combine(Directory.GetCurrentDirectory(), BetterLanguagesPlugin.LocDir());
            if (dir != "")
            {
                FullPath = Path.Combine(FullPath, Dir);
            }
        }

        public void LoadLanguages()
        {
            BetterLanguagesPlugin.log.LogInfo("Reading files in " + FullPath);
            if (!Directory.Exists(FullPath))
            {
                CreateDir();
            }
        }

        private void CreateDir()
        {
            Directory.CreateDirectory(FullPath);
            Assembly asm = Assembly.GetAssembly(typeof(LanguageLoader));
            foreach(string name in asm.GetManifestResourceNames())
            {
                Stream stream = asm.GetManifestResourceStream(name);

                string filename = name.Replace("BetterLanguages.Assets.", "/");

                BetterLanguagesPlugin.log.LogInfo("creating new file at :" + FullPath + filename);
                using (FileStream ResourceFile = new FileStream(FullPath + filename, FileMode.Create))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(ResourceFile);
                    stream.Close();
                }
            }
        }

        public Dictionary<string, LiteLanguageUnit> getLanguagesList()
        {
            Dictionary<string, LiteLanguageUnit> langs = new Dictionary<string, LiteLanguageUnit>();

            foreach (string filePath in Directory.GetFiles(FullPath))
            {
                if (Path.GetExtension(filePath) != ".txt") continue; //only accept txt files
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                LiteLanguageUnit element = new LiteLanguageUnit();

                element.filePath = filePath;
                element.fileName = fileName;
                CultureInfo ci = GetCultureInfo(fileName);
                element.DisplayName = (ci == null ? fileName : ci.NativeName);

                if (!langs.TryAdd(element.DisplayName, element))
                {
                    BetterLanguagesPlugin.log.LogWarning("Duplicate of language:" + element.DisplayName);
                    BetterLanguagesPlugin.log.LogInfo("Discarded new entry.");
                }
            }
            return langs;
        }

        public FullLanguageUnit GetLang(LiteLanguageUnit lite)
        {
            FullLanguageUnit full = null;
            using (StreamReader streamReader = File.OpenText(lite.filePath))
            {
                BetterLanguagesPlugin.log.LogInfo(lite.filePath);
                full = new FullLanguageUnit(streamReader);
            }
            return full;
        }

        public static CultureInfo GetCultureInfo(string name)
        {
            CultureInfo info = null;
            try { info = CultureInfo.GetCultureInfo(name); }
            catch (CultureNotFoundException) { /*I DONT CARE ! I LOVE IT !*/ }
            return info;
        }
    }
}