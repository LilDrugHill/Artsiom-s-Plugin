#if TRIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using LinkCleaner.Storage.Services;

namespace LinkCleaner.Tests
{
    public class Program
    {
        public static void Main()
        {
            string pathDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\..\Temp"));
            string pathFile = Path.Combine(pathDir, "MonitoringConfig.xml");

            MonitoringConfig.PrepareConfigFile(confDir: pathDir, confFile:pathFile);
            Assert.IsTrue(File.Exists(pathFile));
            

            Console.WriteLine();
        }
    }
}
#endif