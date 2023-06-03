using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.IO;
using System.Security;
using System.Diagnostics;

namespace MagmaMc.MagmaSimpleConfig.Utils
{
    public static class AES
    {
        public const string Header = "\n<AES-Encrypted>";
        public static readonly byte[] saltBytes = new byte[] { 77, 97, 103, 109, 97, 77, 99, 0 }; // Unicode -> Decimal (MagmaMc\0)
        public const ushort Strength = 1024;

        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, Strength);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;
                    AES.Padding = PaddingMode.PKCS7;

                    using (var encryptor = AES.CreateEncryptor())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        }
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            //Console.WriteLine($@"AES Encryption Completed With {Strength} Iterations On {encryptedBytes.Length / 1024}KB Of Data.");
            return encryptedBytes;
        }

        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, Strength);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;
                    AES.Padding = PaddingMode.PKCS7;

                    using (var decryptor = AES.CreateDecryptor())
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        }
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            //Console.WriteLine($@"AES Decryption Completed With {Strength} Iterations On {decryptedBytes.Length / 1024}KB Of Data.");
            return decryptedBytes;
        }



        public static void EncryptFile(string file, string Password)
        {
            if (Password == null)
                return;
            byte[] FileContent = File.ReadAllBytes(file);
            if (!FileContent.ToString().EndsWith(Header))
            {
                byte[] passwordBytes = Encoding.ASCII.GetBytes(Password);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                List<byte> bytesEncrypted = AES_Encrypt(FileContent, passwordBytes).ToList();


                bytesEncrypted.AddRange(Encoding.UTF8.GetBytes(Header));
                File.WriteAllBytes(file, bytesEncrypted.ToArray());
                bytesEncrypted.Clear();
            }
        }

        public static byte[] EncryptData(byte[] Data, string Password)
        {
            if (Password == null || Encoding.UTF8.GetString(Data).EndsWith(Header))
                return Data;
            byte[] FileContent = Data;
            byte[] passwordBytes = Encoding.ASCII.GetBytes(Password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            List<byte> bytesEncrypted = AES_Encrypt(FileContent, passwordBytes).ToList();


            Console.WriteLine(Encoding.UTF8.GetString(bytesEncrypted.ToArray()));
            bytesEncrypted.AddRange(Encoding.UTF8.GetBytes(Header));

            return bytesEncrypted.ToArray();
            
        }

        public static void DecryptFile(string fileEncrypted, string Password)
        {
            if (Password == null)
                return;
            if (File.ReadAllText(fileEncrypted).EndsWith(Header))
            {
                byte[] FileOGContent = File.ReadAllBytes(fileEncrypted);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(Password);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
                byte[] FileContent = new byte[FileOGContent.Length - Header.Length];
                Array.Copy(FileOGContent, FileContent, FileOGContent.Length - Header.Length);

                byte[] bytesDecrypted = AES_Decrypt(FileContent, passwordBytes);
                File.WriteAllBytes(fileEncrypted, bytesDecrypted);
            }
        }


        public static byte[] DecryptData(byte[] RawData, string Password)
        {
            if (Password == null || !Encoding.ASCII.GetString(RawData).EndsWith(Header))
                return RawData;

            byte[] FileOGContent = RawData;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(Password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            byte[] FileContent = new byte[FileOGContent.Length - Header.Length];
            Array.Copy(FileOGContent, FileContent, FileOGContent.Length - Header.Length);
            Console.WriteLine(Encoding.ASCII.GetString(AES_Decrypt(FileContent, passwordBytes)));
            return AES_Decrypt(FileContent, passwordBytes);
        }

    }
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
        protected static string GetSection(string Line, string Defualt = null)
        {
            if (Line.StartsWith("[\"") && Line.EndsWith("\"]"))
                return Line.Replace("[\"", "").Replace("\"]", "");

            else if (Line.StartsWith("['") && Line.EndsWith("']"))
                return Line.Replace("['", "").Replace("']", "");

            else if (Line.StartsWith("[") && Line.EndsWith("]"))
                return Line.Replace("[", "").Replace("]", "");

            else
                return Defualt;
        }
        public class Object
        {

            public string Section { get; set; } = "";
            public string Key { get; set; } = "NULL";
            public string Value { get; set; } = "";

            public override string ToString() => Key + " => " + Value;
            
            public override int GetHashCode()
            {
                return (int)((Value.Length ^ 2 + (Section.GetHashCode() - 512)+Key.GetHashCode()) - Key.Length) * 23;
            }

        }


        public Object GetObject(string Line)
        {
            string Eq = "=>";
            string[] strings = Line.Split(new string[] { Eq }, StringSplitOptions.TrimEntries);
            if (Line.Contains(Eq))
            {
                Object item = new Object();
                item.Key = strings[0].Trim();
                item.Value = strings[1].Trim();
                return item;
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
