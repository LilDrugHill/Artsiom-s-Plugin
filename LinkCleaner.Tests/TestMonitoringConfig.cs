using LinkCleaner.Storage.Services;
using System.Reflection;

namespace LinkCleaner.Tests
{
    [TestClass]
    public sealed class TestMonitoringConfig
    {
        [TestMethod]
        public void TestCreatingFile()
        {
            string pathDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\..\TestDir"));
            string pathFile = Path.Combine(pathDir, "MonitoringConfig.xml");

            // Ensure the directory exists
            MonitoringConfig.PrepareConfigFile(confDir: pathDir, confFile: pathFile);
            Assert.IsTrue(File.Exists(pathFile));
            File.Delete(pathFile);
        }

        [TestMethod]
        public void TestCreatingDir()
        {
            string pathDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\..\TestDir\DirDoesNotExists"));
            string pathFile = Path.Combine(pathDir, "MonitoringConfig.xml");

            MonitoringConfig.PrepareConfigFile(confDir: pathDir, confFile: pathFile);
            Assert.IsTrue(Directory.Exists(pathDir), "Dir does not creates");
            Assert.IsTrue(File.Exists(pathFile), "File does not creates");
            Directory.Delete(pathDir, true); // Delete the directory after the test
        }
    }
}
