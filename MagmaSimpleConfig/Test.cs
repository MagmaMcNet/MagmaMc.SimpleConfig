using System;
using System.Collections.Generic;
using System.Text;

namespace MagmaMc.MagmaSimpleConfig.Tester
{
    internal static class Test
    {
        internal static void Main(string[] args)
        {
            SimpleConfig SimpleConfig = new SimpleConfig("example.txt", true);
            float float1 = (float)SimpleConfig.GetValue("float1", 1.0f, "Example");
            SimpleConfig.SetValue("test", "asdasdads", "Example");
            Console.WriteLine(float1);
        }
    }
}
