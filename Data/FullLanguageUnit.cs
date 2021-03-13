using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnhollowerBaseLib;

namespace BetterLanguages.Data
{
    class FullLanguageUnit
    {
		public static readonly string MissingString = "";

		private StringBuilder builder = new StringBuilder(512);

		private Dictionary<string, string> Strings = new Dictionary<string, string>();

		public FullLanguageUnit(StreamReader stringReader)
		{
			for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
			{
				if (text.Length != 0)
				{
					int num = text.IndexOf('\t');
					string key = "";
					if (num < 0)
					{
						BetterLanguagesPlugin.log.LogWarning("Couldn't parse: " + text);
					}
					else
					{
						key = text.Substring(0, num);
						if (!Strings.ContainsKey(key))
						{
							string value = this.UnescapeCodes(text, num + 1);
							Strings.Add(key, value);
							//BetterLanguagesPlugin.log.LogInfo("(key:"+ key +"):" + text);
						}
						else
						{
							BetterLanguagesPlugin.log.LogWarning("Key already exists: " + key);
						}
					}
				}
			}
		}

		public bool GetString(out string text, StringNames stringId, Il2CppReferenceArray<Il2CppSystem.Object> parts)
		{
			string key = Enum.GetName(typeof(StringNames), stringId);
			if (Strings.TryGetValue(key, out text))
			{
				if (parts.Length != 0)
				{
					List<object> parameters = new List<object>();
					foreach(Il2CppSystem.Object obj in parts)
                    {
                        Il2CppSystem.Type t = obj.GetIl2CppType();
                        //BetterLanguagesPlugin.log.LogInfo("hey :" + t.ToString());
                        if (Il2CppSystem.Type.IsIntegerType(t))
                        {
                            parameters.Add(obj.Unbox<System.Int32>());
                        }
                        else
                        {
                            parameters.Add(obj.ToString());
                        }
                    }
					text = string.Format(text, parameters.ToArray());
				}
				return true;
			}
			return false;
		}
		public bool GetString(out string text, StringNames stringId)
		{
			string key = Enum.GetName(typeof(StringNames), stringId);
			if (Strings.TryGetValue(key, out text))
			{
				return true;
			}
			BetterLanguagesPlugin.log.LogInfo("couldn't find key:" + key);
			return false;
		}
		public bool GetString(out string text, string key, params object[] parts)
		{
			if (Strings.TryGetValue(key, out text))
			{
				if (parts.Length != 0)
				{
					text = string.Format(text, parts);
				}
				return true;
			}
			BetterLanguagesPlugin.log.LogInfo("couldn't find key:" + key);
			return false;
		}

		private string UnescapeCodes(string src, int startAt)
		{
			builder.Clear();
			for (int i = startAt; i < src.Length; i++)
			{
				char c = src[i];
				if (c == '\\')
				{
					char c2 = src[++i];
					if (c2 != 'n')
					{
						if (c2 == 't')
						{
							builder.Append('\t');
						}
					}
					else
					{
						builder.Append('\n');
					}
				}
				else
				{
					builder.Append(c);
				}
			}
			return builder.ToString();
		}
    }
}
