using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace MagmaMc.MagmaSimpleConfig.Utils
{
    public class Global: PackageData
    {

        private bool LoggerEnabled { get; set; } = true;
        private bool FallBackEnabled { get; set; } = false;



        /// <summary>
        /// A List Of All Base10 Numbers
        /// </summary>
        protected readonly static string[] Numbers = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };


        /// <summary>
        /// Converts a string value to a compatible C# data type.
        /// Currently supports conversion of "string, double, float, int, bool, array, and dictionary" types.
        /// For arrays, the input string must be enclosed in square brackets and contain comma-separated values.
        /// For dictionaries, the input string must be enclosed in curly braces and contain key-value pairs in the format 'key': value.
        /// </summary>
        /// <param name="Value">The input string to convert.</param>
        /// <returns>The converted value as an object of the appropriate type.</returns>
        protected static object ValueConverter(string @Value)
        {
            if (Value.StartsWith("\"") && Value.EndsWith("\"")) // String
                return Value.TrimStart('"').TrimEnd('"');
            else if (Value.StartsWith("'") && Value.EndsWith("'"))
                return Value.TrimStart('\'').TrimEnd('\'');
            else if (StringContainsAny(Value, Numbers) && Value.ToLower().EndsWith("f")) // Float
                return float.Parse(Value.Replace("f", ""));
            else if (Value.Split('.').Length == 2 && StringContainsAny(Value, Numbers)) // Double
                return double.Parse(Value);
            else if (Value.All(Char.IsNumber) && !Value.All(Char.IsLetter)) // int
                return int.Parse(Value);
            else if (Value.ToLower() == "false" || Value.ToLower() == "true")
                return bool.Parse(Value.ToLower());
            else if (Value.StartsWith("[") && Value.EndsWith("]")) // Array
            {
                string[] elements = Value.Substring(1, Value.Length - 2)
                                        .Split(',')
                                        .Select(s => s.Trim())
                                        .ToArray();
                object[] converted = new object[elements.Length];
                for (int i = 0; i < elements.Length; i++)
                {
                    converted[i] = ValueConverter(elements[i]);
                }
                return converted;
            }
            else if (Value.StartsWith("{") && Value.EndsWith("}")) // Dictionary
            {
                string[] pairs = Value.Substring(1, Value.Length - 2)
                                      .Split(',')
                                      .Select(s => s.Trim())
                                      .ToArray();
                Dictionary<string, object> converted = new Dictionary<string, object>();
                foreach (string pair in pairs)
                {
                    string[] parts = pair.Split(':');
                    string key = parts[0].TrimStart().TrimEnd('\'');
                    string value = parts[1].TrimStart();
                    converted.Add(key, ValueConverter(value));
                }
                return converted;
            }
            else
            {
                return @Value;
            }
        }


        /// <summary>
        /// Checks If A String Contians Any Of The <paramref name="Contain"/>
        /// </summary>
        /// <param name="String"></param>
        /// <param name="Contain"></param>
        /// <returns>true if contained at least once</returns>
        protected static bool StringContainsAny(string String, params string[] Contain)
        {
            foreach (string Line in Contain)
            {
                if (String.Contains(Line))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Enables Or Disables Logger
        /// </summary>
        public void SetLogger() => LoggerEnabled = !LoggerEnabled;

        /// <summary>
        /// Enables Or Disables Logger
        /// </summary>
        public void SetLogger(bool @Set) => LoggerEnabled = @Set;


        /// <summary>
        /// Orange #ff8f2e,
        /// red #ff3030
        /// </summary>
        /// <param name="strings"></param>
        protected void Logger(params string[] strings)
        {
            if (LoggerEnabled)
            {
                string Combined = "";
                foreach (string s in strings) { Combined += s; }
                Console.WriteLine(Combined);
            }
        }


        public void SetFallBackEnabled() => FallBackEnabled = !FallBackEnabled;
        public bool GetFallBackEnabled() => FallBackEnabled;
        public void SetFallBackEnabled(bool @Set) => FallBackEnabled = @Set;

        /// <summary>
        /// Gets The Line Section If Set
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        protected static string GetSection(string Line)
        {
            if (Line.StartsWith("[\"") && Line.EndsWith("\"]"))
                return Line.Replace("[\"", "").Replace("\"]", "");

            else if (Line.StartsWith("['") && Line.EndsWith("']"))
                return Line.Replace("['", "").Replace("']", "");

            else if (Line.StartsWith("[") && Line.EndsWith("]"))
                return Line.Replace("[", "").Replace("]", "");


            else if (Line.StartsWith("<") && Line.EndsWith(">"))
                return Line.Replace("<", "").Replace(">", "");

            else if (Line.StartsWith("<\"") && Line.EndsWith("\">"))
                return Line.Replace("<\"", "").Replace("\">", "");

            else if (Line.StartsWith("<'") && Line.EndsWith("'>"))
                return Line.Replace("<'", "").Replace("'>", "");
            else
                return "";
        }
        protected string GetComment(string Line)
        {
            if (Line.StartsWith("//"))
                return Line.Replace("//", "");
            else
                return Line;
        }
        protected void InsertLineComment(List<string> List, ushort Index, string Comment) =>
            List.Insert(Index, "//" + Comment);

        protected void AddLineComment(List<string> List, string Comment) =>
            List.Add("//" + Comment);
        public class Object
        {

            public string Section { get; set; } = "";
            public string Key { get; set; } = "NULL";
            public object Value { get; set; } = "";
            /*public static bool operator == (Object obj1, Object obj2)
            {
#if NETCOREAPP3_1_OR_GREATER
                Thread.Sleep(1000);
#endif
                if (obj1 == null)
                    return false;
                if (obj2 == null)
                    return false;

                if (obj1.Section != obj2.Section)
                    return false;
                if (obj1.Key != obj2.Key)
                    return false;
                if (obj1.Value != obj2.Value)
                    return false;

                return true;
            }
            public static bool operator != (Object obj1, Object obj2) =>
                !(obj1 == obj2);

            public override bool Equals(object obj2)
            {
                return this == obj2;
            }
            
            public override int GetHashCode()
            {
                return (int)((Value.Length ^ 2 + (Section.GetHashCode() - 512)+Key.GetHashCode()) - Key.Length) * 23;
            }
            */

        }

        public Object GetObject(string Line, string Section)
        {
            string Eq = GetFallBackEnabled() ? "=" : "=>";
            if (GetSection(Line) != "")
            {
                string[] strings = Line.Split(new string[] { Eq }, StringSplitOptions.None);
                if (strings.Contains(Eq))
                {
                    Object item = new Object();
                    item.Section = Section;
                    item.Key = strings[0].Trim();
                    if (strings[1].StartsWith("[") && strings[1].EndsWith("]"))
                    {
                        // array value
                        item.Value = strings[1].Trim('[', ']').Split(',').Select(s => ValueConverter(s.Trim())).ToArray();
                    }
                    else
                    {
                        // normal value
                        item.Value = ValueConverter(strings[1].Trim());
                    }
                    return item;
                }
            }
            return null;
        }



        internal static string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
    public class PackageData
    {
        public const string Version = "1.0.0";
        public const string SupportedFiles = "INI, TOML, MSC";
    }
}
