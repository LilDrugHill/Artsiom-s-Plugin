using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using LinkCleaner.Storage.Services;
using LinkCleaner.Presentation.Models;


namespace LinkCleaner.Tests
{
    [TestClass]
    public sealed class TestMonitoringConfig
    {
        [TestInitialize]
        public void TestInitialize()
        {
            MonitoringConfig.ClearCache();
        }

        [TestMethod]
        public void TestCreatingFile()
        {
            string pathDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\..\TestDir"));
            string pathFile = Path.Combine(pathDir, "MonitoringConfig.xml");

            // Ensure the directory exists
            MonitoringConfig.PrepareConfigFile(confFile: pathFile);
            Assert.IsTrue(File.Exists(pathFile));
            //File.Delete(pathFile);
        }

        [TestMethod]
        public void TestCreatingDir()
        {
            string pathDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\..\TestDir\DirDoesNotExists"));
            string pathFile = Path.Combine(pathDir, "MonitoringConfig.xml");

            MonitoringConfig.PrepareConfigFile(confFile: pathFile);
            Assert.IsTrue(Directory.Exists(pathDir), "Dir does not creates");
            Assert.IsTrue(File.Exists(pathFile), "File does not creates");
            Directory.Delete(pathDir, true); // Delete the directory after the test
        }

        [TestMethod]
        [DataRow("BadConfig1.xml")]
        [DataRow("BadConfig2.xml")]
        [DataRow("BadConfig3.xml")]
        [DataRow("BadConfig4.xml")]
        [DataRow("BadConfig5.xml")]
        public void TestConfigValidationBadConfig(string confFileName)
        {
            string dirPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\..\TestDir\Templates"));
            string filePath = Path.Combine(dirPath, confFileName);

            Assert.IsFalse(MonitoringConfig.TryGetValidXmlConfig(filePath, out XmlDocument xmlDoc));
            Assert.IsTrue(xmlDoc is null);


        }

        [TestMethod]
        [DataRow("GoodConfig1.xml")]
        [DataRow("GoodConfig2.xml")]
        [DataRow("GoodConfig3.xml")]
        public void TestConfigValidationGoodConfig(string confFileName)
        {
            string dirPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\..\TestDir\Templates"));
            string filePath = Path.Combine(dirPath, confFileName);

            Assert.IsTrue(MonitoringConfig.TryGetValidXmlConfig(filePath, out XmlDocument? xmlDoc));
            Assert.IsTrue(xmlDoc is XmlDocument);
        }

        [TestMethod]
        public void TestConfigDeserialization()
        {
            string pathDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\..\TestDir"));
            string pathFile = Path.Combine(pathDir, "MonitoringConfig.xml");

            List<DocumentModelInWPF> documents = MonitoringConfig.DeserializeConfig(pathFile);
            Assert.AreEqual(documents.Count, 2);
            Assert.AreEqual(documents[0].LinksInDocument.Count, 3);
        }

        [TestMethod]
        public void TestConfigSerialization()
        {
            string pathDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\..\TestDir"));
            string pathFile1 = Path.Combine(pathDir, "MonitoringConfigNEW.xml");
            string pathFile2 = Path.Combine(pathDir, "MonitoringConfig2.xml");

            List<DocumentModelInWPF> documents = MonitoringConfig.DeserializeConfig(pathFile2);
            byte fCount = (byte)documents.Count;
            documents.Add(new DocumentModelInWPF("Test5", new Guid("59a75bfe-73d9-4e6b-a2a6-9475b3ddbcf4"), true));
            MonitoringConfig.SerializeConfig(pathFile1);
            MonitoringConfig.ClearCache();

            List<DocumentModelInWPF> documentsNew = MonitoringConfig.DeserializeConfig(pathFile1);
            Assert.AreEqual(fCount, documentsNew.Count);

            
            File.Delete(pathFile1); // Clean up after test
        }
    }
}
