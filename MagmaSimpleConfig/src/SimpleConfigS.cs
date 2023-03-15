using System.Linq;
using System;
using System.Collections.Generic;
using MagmaMc.MagmaSimpleConfig.Utils;

namespace MagmaMc.MagmaSimpleConfig
{
    /// <summary>
    /// String Version Of <seealso cref="SimpleConfig"/>
    /// </summary>
    [Obsolete("Not Finished Please Use At Your Own Risk")]
    public class SimpleConfigS: Global
    {
        public List<string> ConfigData { get; private set; }


#pragma warning disable CS0618 // Disable Obsolete Warning These Are The Built In Function

        /// <summary>
        /// String Version Of <seealso cref="SimpleConfig"/>
        /// </summary>
        public SimpleConfigS(string data)
        {
            ConfigData = data.Split('\n').ToList();
        }




        /// <summary>
        /// It sets the value of a key in a section
        /// </summary>
        /// <param name="Key">The key of the value you want to set</param>
        /// <param name="Value">The value you want to set the key to.</param>
        /// <param name="Section">The section of the file you want to set the value in.</param>
        /// <param name="UseFallback">If true, the value will be set to the key with a " = " instead of a " => ".</param>
        /// <exception cref="ArgumentException"></exception>
        public void SetValue(string Key, object Value, string Section = "", bool UseFallback = false)
        {
            if (Key == "")
                throw new ArgumentException("Key Can Not Equal \"\"");
            if (UseFallback && !GetFallBackEnabled())
                throw new ArgumentException("Fallback Is Not Enabled");
            int Index = 0;
            string CurrentSection = "";
            List<string> Sections = new List<string>();
            foreach (string Line in ConfigData)
            {
                CurrentSection = (GetSection(Line) == "" ? CurrentSection : GetSection(Line));
                Object @object = GetObject(Line, CurrentSection);

                if (!Sections.Contains(CurrentSection))
                    Sections.Add(CurrentSection);
                if (Line != "" && Line != "\n" && Line != " " && !Line.StartsWith("//") && GetSection(Line) == "")
                {
                    if (CurrentSection == Section)
                    {
                        if (Line.Split(' ')[0].Trim() == Key)
                        {
                            ConfigData[Index] = Key + (UseFallback ? (" = ") : (" => ")) + Value;
                            return;
                        }
                    }
                }

                Index++;
            }
            if (!Sections.Contains(Section))
            {
                try
                {
                    if (ConfigData.Last() == "")
                        Index -= 1;
                }
                catch { }
                try
                {
                    if (ConfigData[ConfigData.Count - 1] == "" || ConfigData[ConfigData.Count - 1] == " ")
                    {
                        ConfigData[ConfigData.Count - 1] = "[" + Section + "]\n" + Key + (UseFallback ? (" = ") : (" => ")) + Value;
                        //Lines[Index] = "\n<" + Section + ">\n" + Key + (UseFallback ? (" = ") : (" => ")) + Value;
                    }
                }
                catch
                {
                    ConfigData.Add("[" + Section + "]\n" + Key + (UseFallback ? (" = ") : (" => ")) + Value);

                }
            }
            else
            {
                Index = 0;
                CurrentSection = "";
                foreach (string Line in ConfigData)
                {
                    CurrentSection = (GetSection(Line) == "" ? CurrentSection : GetSection(Line));

                    if (CurrentSection == Section)
                    {
                        while (GetSection(ConfigData[Index]) != "")
                            Index++;
                        if (ConfigData[Index - 1] == "")
                            Index -= 1;
                        ConfigData[Index] = ConfigData[Index] + "\n" + Key + (UseFallback ? (" = ") : (" => ")) + Value;
                        break;
                    }


                    Index++;
                }
            }

        }

        /// <summary>
        /// Get Value Of Key From "Magma's Simple Config Format"
        /// </summary>
        /// <param name="Key">The String Name Of The Variable Name To Locate Eg</param>
        /// <param name="DefualtValue">Defualt Value Of Key If Not Found Or Not Set</param>
        /// <param name="Section"></param>
        /// <returns></returns>
        public object GetValue(string Key, object DefualtValue = null, string Section = "")
        {
            string CurrentSection = "";
            int index = 0;
            foreach (string Line in ConfigData)
            {
                index++;
                // Section Finder
                CurrentSection = (GetSection(Line) == "" ? CurrentSection : GetSection(Line));


                // Key > Value | Checker
                if (CurrentSection == Section && Line.StartsWith(Key))
                {
                    if (Line.StartsWith(Key + "=>"))
                        return ValueConverter(Line.Replace(Key + "=>", ""));
                    else if (Line.StartsWith(Key + " => "))
                        return ValueConverter(Line.Replace(Key + " => ", ""));


                    // Key = Value | Checker  -- Fallback If Someone Tries To Enter It Wrongly
                    if (Line.Contains("=") && !Line.Contains("=>") && GetFallBackEnabled())
                    {

                        Logger($"Fallback Usage of '=' On Line {index}, Please Use '=>' Or Disable Logger");
                        if (Line.StartsWith(Key + "="))
                            return ValueConverter(Line.Replace(Key + "=", ""));
                        else if (Line.StartsWith(Key + " = "))
                            return ValueConverter(Line.Replace(Key + " = ", ""));
                    }
                }
            }
            return DefualtValue ?? "";
        }


        public static string ConvertToData(string[] strings)
        {
            string ReturnData = string.Empty;
            foreach(string line in strings)
                ReturnData += line;
            return ReturnData;
        }

        public static string ConvertToData(List<string> strings) => ConvertToData(strings.ToArray());


#pragma warning restore CS0618 // :35
    }
}
