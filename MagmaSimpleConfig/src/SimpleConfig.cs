using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using MagmaMc.MagmaSimpleConfig.Utils;
using System.Text;
using System.Diagnostics;

namespace MagmaMc.MagmaSimpleConfig
{
    public class SimpleConfig: Global
    {
        public string FileName { get; }
        public bool AutoGenerate { get; } = true;
        public string AESPassword = null;


        public SimpleConfig(string FileName)
        {
            this.FileName = FileName;
        }

        public SimpleConfig(string FileName, string AESPassword)
        {
            this.FileName = FileName;
            this.AESPassword = AESPassword;
        }



        /// <summary>
        /// If the file exists, read it. If it doesn't, create it and then read it
        /// </summary>
        /// <returns>
        /// The file is being read and returned as a string array.
        /// </returns>
        public string[] ReadWithCatch()
        {
            if (!File.Exists(FileName))
                File.WriteAllText(FileName, "");
            return Encoding.ASCII.GetString(AES.DecryptData( File.ReadAllBytes(FileName), AESPassword)).Split(new string[] { Environment.NewLine },    StringSplitOptions.None);
        }

        public string GetValue(string Key, string Section = null, string Default = null)
        {
            string CurrentSection = null;
            if (Section == null)
                foreach (string Line in ReadWithCatch())
                {
                    if (GetSection(Line, null) != null)
                        return null;

                    Object @object = GetObject(Line);

                    if (@object.Key == Key)
                        return @object.Value;
                }
            else
                foreach (string Line in ReadWithCatch())
                {
                    CurrentSection = GetSection(Line, CurrentSection);
                    if (CurrentSection != Section)
                        continue;

                    Object @object = GetObject(Line);

                    if (@object.Key == Key)
                        return @object.Value;
                }

            return Default;
        }

        public void SetValue(string Key, string Value, string Section = null)
        {
            List<string> Lines = ReadWithCatch().ToList();
            string CurrentSection = null;
            if (!Lines.Any(line => line.StartsWith(Key)))
            {
                // if User Supplied No Section
                if (Section == null)
                {
                    Lines[0] = Key + " => " + Value + "\r\n" + Lines[0];
                    File.WriteAllBytes(FileName, AES.EncryptData(Encoding.ASCII.GetBytes(string.Join(Environment.NewLine, Lines.ToArray())), AESPassword));
                    return;
                }
                // if key exists
                int Index = 0;
                foreach(string Line in Lines)
                {
                    CurrentSection = GetSection(Line, CurrentSection);
                    if (CurrentSection != Section)
                        continue;
                    Object value = GetObject(Line);
                    if (value != null)
                        if (value.Key == Key)
                        {
                            Lines[Index] = value.ToString();
                            File.WriteAllBytes(FileName, AES.EncryptData(Encoding.ASCII.GetBytes(string.Join(Environment.NewLine, Lines.ToArray())), AESPassword));
                            return;
                        }
                    Index++;
                }

                Index = -1;
                bool InSection = false;
                CurrentSection = null;
                foreach (string Line in ReadWithCatch())
                {
                    Index++;
                    if (CurrentSection != Section)
                    {
                        CurrentSection = GetSection(Line, CurrentSection);
                        if (CurrentSection == Section)
                            InSection = true;
                        if (InSection)
                        {
                            Lines[Index+1] = Key + " => " + Value + "\r\n" + Lines[Index+1];
                            File.WriteAllBytes(FileName, AES.EncryptData(Encoding.ASCII.GetBytes(string.Join(Environment.NewLine, Lines.ToArray())), AESPassword));
                            return;
                        }
                        continue;
                    }
                    CurrentSection = GetSection(Line, CurrentSection);
                    InSection = true;
                    Object value = GetObject(Line);
                    if (value != null)
                        if (value.Key == Key)
                        {
                            Lines[Index] = value.ToString();
                            File.WriteAllBytes(FileName, AES.EncryptData(Encoding.ASCII.GetBytes(string.Join(Environment.NewLine, Lines.ToArray())), AESPassword));
                            return;
                        }
                }
                if (!InSection)
                {
                    if (string.IsNullOrEmpty(Lines[Lines.Count - 1]))
                        Lines[Lines.Count - 1] = $"[{Section}]\r\n{Key} => {Value}";
                    else
                        Lines.Add($"[{Section}]\r\n{Key} => {Value}");
                }
                File.WriteAllBytes(FileName, AES.EncryptData(Encoding.ASCII.GetBytes(string.Join(Environment.NewLine, Lines.ToArray())), AESPassword));
            } else
            {

                int Index = -1;
                foreach (string Line in Lines)
                {
                    Index++;
                    CurrentSection = GetSection(Line, CurrentSection);
                    if (CurrentSection != Section)
                        continue;
                    Object value = GetObject(Line);
                    if (value != null)
                        if (value.Key == Key)
                        {
                            value.Value = Value;
                            Lines[Index] = value.ToString();
                            File.WriteAllBytes(FileName, AES.EncryptData(Encoding.ASCII.GetBytes(string.Join(Environment.NewLine, Lines.ToArray())), AESPassword));
                            return;
                        }
                }

                Index = -1;
                CurrentSection = null;
                foreach (string Line in ReadWithCatch())
                {
                    Index++;
                    CurrentSection = GetSection(Line, CurrentSection);
                    Object value = GetObject(Line);
                    if (value != null)
                        if (value.Key == Key)
                        {
                            Lines[Index] = value.ToString();
                            File.WriteAllBytes(FileName, AES.EncryptData(Encoding.ASCII.GetBytes(string.Join(Environment.NewLine, Lines.ToArray())), AESPassword));
                            return;
                        }
                }
                File.WriteAllBytes(FileName, AES.EncryptData(Encoding.ASCII.GetBytes(string.Join(Environment.NewLine, Lines.ToArray())), AESPassword));
            }

        }

    }
}
