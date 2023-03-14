using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using MagmaMc.MagmaSimpleConfig.Utils;

namespace MagmaMc.MagmaSimpleConfig
{

    public class SimpleConfig : Global
    {
        public string FileName { get; }
        public bool AutoGenerate { get; } = true;


#pragma warning disable CS0618 // Disable Obsolete Warning These Are The Built In Function


        public SimpleConfig(string @FileName)
        {
            this.FileName = @FileName;
        }

        public SimpleConfig(string @FileName, bool @AutoGenerateFile = true)
        {
            this.FileName = @FileName;
            this.AutoGenerate = @AutoGenerateFile;
        }



        public string[] ReadWithCatch()
        {
            try
            {
                return File.ReadAllLines(FileName);
            }
            catch
            {
                if (!File.Exists(FileName))
                    File.WriteAllText(FileName, "");
                return File.ReadAllLines(FileName);

            }
        }
        public void AddComment(string Comment, string Key, string Section = "")
        {
            string[] Lines = ReadWithCatch();
            string CurrentSection = "";
            int index = 0;
            CurrentSection = "";
            foreach (string Line in Lines)
            {
                CurrentSection = (GetSection(Line) == "" ? CurrentSection : GetSection(Line));

                index++;
            }
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
        public void SetValue(string Key, object Value, string Section = "", bool UseFallback = false)
        {
            if (Key == "")
                throw new ArgumentException("Key Can Not Equal \"\"");
            if (UseFallback && !GetFallBackEnabled())
                throw new ArgumentException("Fallback Is Not Enabled");
            int Index = 0;
            string[] Lines = ReadWithCatch();
            bool set = false;
            string CurrentSection = "";
            List<string> Sections = new List<string>();
            foreach (string Line in Lines)
            {
                CurrentSection = (GetSection(Line) == "" ? CurrentSection : GetSection(Line));
                Object @object = GetObject(Line, CurrentSection);

                if (!Sections.Contains(CurrentSection))
                    Sections.Add(CurrentSection);

                if (@object.Key == Key && CurrentSection == Section)
                {
                    Lines[Index] = Key + (UseFallback ? (" = ") : (" => ")) + Value;
                    set = true;
                    break;
                }


                Index++;
            }
            if (!set)
            {
                if (!Sections.Contains(Section))
                    File.AppendAllText(FileName, "\n<" + Section + ">\n" + Key + (UseFallback ? (" = ") : (" => ")) + Value);
                else
                {
                    Index = 0;
                    CurrentSection = "";
                    foreach (string Line in Lines)
                    {
                        CurrentSection = (GetSection(Line) == "" ? CurrentSection : GetSection(Line));

                        if (CurrentSection == Section)
                        {
                            while (GetSection(Lines[Index]) != "")
                                Index++;
                            Lines[Index] = Lines[Index] + "\n" + Key + (UseFallback ? (" = ") : (" => ")) + Value;
                            break;
                        }


                        Index++;
                    }
                    File.WriteAllLines(this.FileName, Lines);
                }
            }
            else
                File.WriteAllLines(this.FileName, Lines);
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
            string[] Lines = ReadWithCatch();
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
            return DefualtValue;
        }





#pragma warning restore CS0618 // :35

    }
    [Obsolete("Please Use SimpleConfig Or SimpleConfigS For Strings", true)]
    public class MagmaSimpleConfig
    {
        
        [Obsolete("Please Use SimpleConfig Or SimpleConfigS For Strings", true)]
        public MagmaSimpleConfig(string REMOVE) { }
    }

}
