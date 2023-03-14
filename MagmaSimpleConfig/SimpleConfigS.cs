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
        public string ConfigData { get; private set; }


#pragma warning disable CS0618 // Disable Obsolete Warning These Are The Built In Function

        /// <summary>
        /// String Version Of <seealso cref="SimpleConfig"/>
        /// </summary>
        public SimpleConfigS(string data)
        {
            ConfigData = data;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <param name="Section"></param>
        /// <param name="UseFallback"></param>
        /// <param name="UseSpaces"></param>
        /// <exception cref="ArgumentException"></exception>
        public void SetValue(string Key, object Value, string Section = "", bool UseFallback = false, bool UseSpaces = true)
        {
            if (Key == "")
                throw new ArgumentException("Key Can Not Equal \"\"");
            if (UseFallback && !GetFallBackEnabled())
                throw new ArgumentException("Fallback Is Not Enabled");
            int Index = 0;
            string[] Lines = ConfigData.Replace("\r\n", "\n").Split('\n');
            bool set = false;
            string CurrentSection = "";
            List<string> Sections = new List<string>();
            foreach (string Line in Lines)
            {
                CurrentSection = (GetSection(Line) == "" ? CurrentSection : GetSection(Line));
                if (!Sections.Contains(CurrentSection))
                    Sections.Add(CurrentSection);

                if (Line.StartsWith(Key) && CurrentSection == Section)
                {
                    if (UseFallback)
                        Lines[Index] = Key + (UseSpaces ? (" = ") : ("=")) + Value;
                    else
                        Lines[Index] = Key + (UseSpaces ? (" => ") : ("=>")) + Value;
                    set = true;
                    break;
                }


                Index++;
            }
            if (!set)
            {
                if (!Sections.Contains(Section))
                {
                    if (UseFallback)
                        ConfigData += "\n[" + Section + "]\n" + Key + (UseSpaces ? (" = ") : ("=")) + Value;
                    else
                        ConfigData += "\n[" + Section + "]\n" + Key + (UseSpaces ? (" => ") : ("=>")) + Value;

                }
                else
                {
                    Index = 0;
                    CurrentSection = "";
                    foreach (string Line in Lines)
                    {
                        CurrentSection = (GetSection(Line) == "" ? CurrentSection : GetSection(Line));

                        if (CurrentSection == Section)
                        {
                            while ((Lines[Index].Contains("[") || Lines[Index].Contains("<")) && Lines[Index] != "")
                                Index++;
                            if (UseFallback)
                                Lines[Index] = Lines[Index] + "\n" + Key + (UseSpaces ? (" = ") : ("=")) + Value;
                            else
                                Lines[Index] = Lines[Index] + "\n" + Key + (UseSpaces ? (" => ") : ("=>")) + Value;
                            break;
                        }


                        Index++;
                    }
                    ConfigData = ConvertToData(Lines);
                }
            }
            else
                ConfigData = ConvertToData(Lines);
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
            string[] Lines = ConfigData.Replace("\r\n", "\n").Split('\n');
            string CurrentSection = "";
            int index = 0;
            foreach (string Line in Lines)
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
