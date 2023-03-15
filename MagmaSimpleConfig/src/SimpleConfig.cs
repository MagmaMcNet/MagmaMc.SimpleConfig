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



        /// <summary>
        /// If the file exists, read it. If it doesn't, create it and then read it
        /// </summary>
        /// <returns>
        /// The file is being read and returned as a string array.
        /// </returns>
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

        /// <summary>
        /// It adds a comment to a specific key in a specific section
        /// </summary>
        /// <param name="Comment">The comment to add</param>
        /// <param name="Key">The key of the object you want to add a comment to.</param>
        /// <param name="Section">The section of the ini file you want to add the comment to.</param>
        public void AddComment(string Comment, string Key, string Section = "")
        {
            List <string> Lines = ReadWithCatch().ToList();
            string CurrentSection = "";
            int index = 0;
            CurrentSection = "";
            foreach (string Line in Lines)
            {
                index++;
                CurrentSection = (GetSection(Line) == "" ? CurrentSection : GetSection(Line));
                if (CurrentSection == Section)
                {
                    Object @object = GetObject(Line, CurrentSection);
                    if (Line != "" && Line != "\n" && Line != " " && !Line.StartsWith("//") && GetSection(Line) == "")
                        if (Line.Split(' ')[0].Trim().ToLower() == Key.ToLower())
                        {
                            if (Lines[index-2] != "//" + Comment)
                                Lines.Insert(index-1, "//" + Comment);
                            break;
                        }

                }
            }
            File.WriteAllLines(FileName, Lines.ToArray());
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
            List<string> Lines = ReadWithCatch().ToList();
            string CurrentSection = "";
            List<string> Sections = new List<string>();
            foreach (string Line in Lines)
            {
                CurrentSection = (GetSection(Line) == "" ? CurrentSection : GetSection(Line));
                Object @object = GetObject(Line, CurrentSection);

                if (!Sections.Contains(CurrentSection))
                    Sections.Add(CurrentSection);
                if (Line != "" && Line != "\n" && Line != " " && !Line.StartsWith("//") && GetSection(Line) == "")
                {
                    if (CurrentSection == Section)
                    {
                        if (Line.Split(' ')[0].Trim() == Key )
                        {
                            Lines[Index] = Key + (UseFallback ? (" = ") : (" => ")) + Value;
                            File.WriteAllLines(FileName, Lines.ToArray());
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
                    if (Lines.Last() == "")
                        Index -= 1;
                }
                catch { }
                try
                {
                    if (Lines[Lines.Count - 1] == "" || Lines[Lines.Count - 1] == " ")
                    {
                        Lines[Lines.Count - 1] = "[" + Section + "]\n" + Key + (UseFallback ? (" = ") : (" => ")) + Value;
                        //Lines[Index] = "\n<" + Section + ">\n" + Key + (UseFallback ? (" = ") : (" => ")) + Value;
                    }
                }
                catch
                {
                        Lines.Add("[" + Section + "]\n" + Key + (UseFallback ? (" = ") : (" => ")) + Value);

                }
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
                        while (GetSection(Lines[Index]) != "")
                            Index++;
                        if (Lines[Index - 1] == "")
                            Index -= 1;
                        Lines[Index] = Lines[Index] + "\n" + Key + (UseFallback ? (" = ") : (" => ")) + Value;
                        break;
                    }


                    Index++;
                }
            }
            
            File.WriteAllLines(this.FileName, Lines.ToArray());
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

        /// <summary>
        /// adds A section to the end of the config
        /// </summary>
        /// <param name="Section">The section you want to create.</param>
        public void CreateSection(string Section)
        {
            List<string> Lines = ReadWithCatch().ToList();
            Lines.Add("["+ Section+"]");
            File.WriteAllLines(FileName, Lines.ToArray());
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
