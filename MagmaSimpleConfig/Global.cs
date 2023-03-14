using System.Linq;
using System;
using System.Collections.Generic;

namespace MagmaMc.MagmaSimpleConfig.Utils
{
    public class Global
    {

        private bool LoggerEnabled { get; set; } = true;
        private bool FallBackEnabled { get; set; } = false;



        /// <summary>
        /// A List Of All Base10 Numbers
        /// </summary>
        protected readonly static string[] Numbers = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };


        /// <summary>
        /// Currently Can Only Convert OBJECT To "string, double, float, int, bool"
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        protected static object ValueConverter(string @Value)
        {
            if ((Value.StartsWith("\"") && Value.EndsWith("\""))) // String
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
            public string Section { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public Object GetObject(string Line, string Section)
        {
            string Eq = GetFallBackEnabled() ? "=" : "=>";
            if (GetSection(Line) != "")
            {
                string[] strings = Line.Split(new string[] { Eq }, StringSplitOptions.None);
                if (strings.Contains(Eq))
                {
                    if (strings[1] == Eq)
                    {
                        Object item = new Object();
                        item.Section = Section;
                        item.Key = strings[0];
                        strings[0] = "";
                        strings[1] = "";
                        item.Value = string.Join(" ", strings);
                    }
                }
            }
            return new Object();
        }

    }
    public static class PackageData
    {

        public const string Version = "0.3.0";
        public const string SupportedFiles = "INI, TOML, MSC";
    }
}
