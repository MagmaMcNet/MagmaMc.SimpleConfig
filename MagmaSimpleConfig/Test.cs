using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MagmaMc.MagmaSimpleConfig;
using MagmaMc.MagmaSimpleConfig.Utils;

namespace MagmaMc.MagmaSimpleConfig.Tester
{
    internal static class Test
    {
        internal static void Main(string[] args)
        {
            SimpleConfig ConfigEn = new SimpleConfig("testEn.msc", "MMC");
            SimpleConfig Config = new SimpleConfig("test.msc");
            //Config.SetValue("Test", "test");
            //Config.SetValue("Test2", "test", "sectionname");
            //Config.SetValue("Test3", "test", "sectionname");
            //Config.SetValue("Test4", "test");
            //Config.SetValue("Test5", "test", "sectionname2");
            //Config.SetValue("Test6", "test", "sectionname2");

            Config.SetValue("Test", "test2");
            Config.SetValue("Test2", "test2", "sectionname");
            Config.SetValue("Test3", "test2", "sectionname");
            Config.SetValue("Test4", "test2");
            Config.SetValue("Test5", "test2", "sectionname2");
            Config.SetValue("Test6", "test2", "sectionname2");


            //ConfigEn.SetValue("Test", "test1");
            //ConfigEn.SetValue("Test2", "test", "sectionname");
            //ConfigEn.SetValue("Test3", "test", "sectionname");
            //ConfigEn.SetValue("Test4", "test");
        }
    }
}
