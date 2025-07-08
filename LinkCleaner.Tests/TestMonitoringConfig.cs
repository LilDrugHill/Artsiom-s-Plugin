using LinkCleaner.Presentation.Models;
using LinkCleaner.Storage.Services;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;


namespace LinkCleaner.Tests
{
    [TestClass]
    public sealed class TestMonitoringConfig
    {
        static string PathDir = Path.GetFullPath(Path.Combine(Path.GetTempPath(),
                                                              @"\TestDir"));

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {

            try { Directory.CreateDirectory(PathDir); } catch { /* ignore */ };
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("Project",
                    new XAttribute("Name", "TestProject"),
                    new XAttribute("Guid", "59a75bfe-73d9-4e6b-a2a6-9475b3ddbcf4"),
                    new XAttribute("Status", "Enable"),
                    new XElement("Link",
                        new XAttribute("Name", "TestLink1"),
                        new XAttribute("Guid", "59a75bfe-73d9-4e6b-a2a6-9475b3ddbcf4")
                    ),
                    new XElement("Link",
                        new XAttribute("Name", "TestLink2"),
                        new XAttribute("Guid", "59a75bfe-73d9-4e6b-a2a6-9475b3ddbcf4")
                    ),
                    new XElement("Link",
                        new XAttribute("Name", "TestLink3"),
                        new XAttribute("Guid", "59a75bfe-73d9-4e6b-a2a6-9475b3ddbcf4")
                    )
                )
            );
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(false),
                OmitXmlDeclaration = false // <-- Важно!
               
            };

            using (var writer = XmlWriter.Create(Path.GetFullPath(Path.Combine(PathDir, "59a75bfe-73d9-4e6b-a2a6-9475b3ddbcf4.xml")), settings))
            {
                doc.Save(writer);
            }
        }
        [ClassCleanup]
        public static void CleanupAfterAllTests()
        {
            void TryDelete(string path)
            {
                if (File.Exists(path))
                {
                    try { File.Delete(path); } catch { /* ignore */ }
                }
            }

            TryDelete(Path.Combine(PathDir, "59a75bfe-73d9-4e6b-a2a6-9475b3ddbcf4.xml"));
            TryDelete(Path.Combine(PathDir, "a87274e2-4956-4344-8b98-e23b5f0733d5.xml"));

            try { Directory.Delete(PathDir, true); } catch { }
        }



        [TestMethod]
        public void TestConfigDeserialization()
        {
            MonitoringConfig mc = new(new Guid("59a75bfe-73d9-4e6b-a2a6-9475b3ddbcf4"), PathDir);

            DocumentModelInWPF? doc = mc.Document;
            Assert.IsNotNull(doc);
            Assert.AreEqual(doc.LinksInDocument.Count, 3);
            mc = null;
        }

        [TestMethod]
        public void TestConfigSerialization()
        {


            MonitoringConfig mc = new(new Guid("a87274e2-4956-4344-8b98-e23b5f0733d5"), PathDir);

            DocumentModelInWPF? doc = mc.Document;
            Assert.IsNull(doc.Name);
            Assert.AreEqual(doc.LinksInDocument.Count, 0);
            Assert.AreEqual(doc.Guid.ToString(), "a87274e2-4956-4344-8b98-e23b5f0733d5");

            doc.Name = "TestName";
            doc.Status = true;
            doc.LinksInDocument.Add(new LinkModelInWPF("TestLink",
                                                       new Guid("a87274e2-4956-4344-8b98-e23b5f0733d5"),
                                                       true));
            mc.SerializeConfig();

            MonitoringConfig mc2 = new(new Guid("a87274e2-4956-4344-8b98-e23b5f0733d5"), PathDir);
            Assert.IsNotNull(mc2.Document?.Name);
            Assert.AreEqual(mc2.Document.LinksInDocument?.Count, 1);
            Assert.AreEqual(mc2.Document.Guid.ToString(), "a87274e2-4956-4344-8b98-e23b5f0733d5");
            Assert.AreEqual(mc2.Document.Status, true);
            Assert.AreEqual(mc2.Document?.LinksInDocument?[0].Name, "TestLink");


        }


    }
}
