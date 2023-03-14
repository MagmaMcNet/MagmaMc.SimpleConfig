using System;
using System.Collections.Generic;
using System.Text;

namespace MagmaMc.MagmaSimpleConfig
{
    internal static class Test
    {
        internal static void Main(string[] args)
        {
            SimpleConfig simpleConfig = new SimpleConfig("example.txt");
            float float1 = (float)simpleConfig.GetValue("float1", 1.0f, "Example");
            Console.WriteLine($"{float1} float 1 1.0f");
            float float2 = float.Parse(simpleConfig.GetValue("float2", 1.0f, "Example").ToString());
            Console.WriteLine($"{float2} float 2 1.0");
            float float3 = (float)simpleConfig.GetValue("float3", 1.0f, "Example");
            Console.WriteLine($"{float3} float 2 1f");
        }
    }
}
