using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using LinkCleaner.Presentation.Emuns;
using LinkCleaner.Presentation.Models;
using LinkCleaner.Storage.Interfaces;


namespace LinkCleaner.Storage.Services
{
    public class MonitoringConfig : IConfigurationStorage
    {
        XmlDocument ConfigurationDocument;
        
        public string ConfigFilePath { get; init; }
        public string ConfigDirPath { get; init; }

        DocumentModelInWPF? _document;

        public DocumentModelInWPF? Document { get => _document; private set => _document = value; }

        public MonitoringConfig(Guid projectGuid, string? confDir = null)
        {
            ConfigDirPath = confDir ?? IConfigurationStorage.PathToConfigurationDirectory;
            ConfigFilePath = Path.Combine(ConfigDirPath, $"{projectGuid.ToString()}.xml");

            DeserializeConfig(projectGuid);
        }



        /// <summary>
        /// Создает конфигурационный файл если это требуется. Параметры нужны ТОЛЬКО для тестирования 
        /// </summary>
        /// <param name="confDir"></param>
        /// <param name="confFile"></param>
        public void PrepareConfigFile()
        {
            try
            {
                switch (Directory.Exists(ConfigDirPath), File.Exists(ConfigFilePath))
                {
                    case (false, false):
                        Directory.CreateDirectory(ConfigDirPath);
                        CreateConfFile(ConfigFilePath);
                        InitConfDoc(ConfigFilePath);
                        break;
                    case (true, false):
                        CreateConfFile(ConfigFilePath);
                        InitConfDoc(ConfigFilePath);
                        break;
                    case (true, true):
                        if (!TryGetValidXmlConfig(ConfigFilePath, out XmlDocument? xmlDoc))
                        {
                            File.Delete(ConfigFilePath);
                            CreateConfFile(ConfigFilePath);
                            InitConfDoc(ConfigFilePath);
                        } else
                        {
                            InitConfDoc(xmlDoc);
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                //TaskDialog.Show("Error", $"Failed to create config file: {ex.Message}.");
            }
        }

        void InitConfDoc(string confFile) 
        {
                var doc = new XmlDocument();
                doc.LoadXml(confFile);
                ConfigurationDocument = doc;
        }
        void InitConfDoc(XmlDocument xmlDoc)
        {
            ConfigurationDocument = xmlDoc;
        }

        void CreateConfFile(string path)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement(IConfigurationStorage.RootName));
            doc.Save(path);
        }



        // TODO: Refactor - malek po-debilnomy sdelano
        /// <summary>
        /// If it returns true. out set loaded XML configuration. If false out is null.
        /// </summary>
        /// <param name="confFile"></param>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public bool TryGetValidXmlConfig(string confFile, out XmlDocument? xmlDoc) 
        {

            if (File.Exists(confFile))
            {
                if (!ValidateMetaInfo(confFile))
                {
                    xmlDoc = null;
                    return false; // Wrong MetaData in XML file
                }
                xmlDoc = new();

                try
                {
                    xmlDoc.Load(confFile);
                    XmlNode root = xmlDoc.DocumentElement;
                    if (root is XmlElement xmlRoot && xmlRoot.Name == IConfigurationStorage.RootName
                        && xmlRoot.Attributes.GetNamedItem(IConfigurationStorage.DocNameField)?.Value is not null
                        && xmlRoot.Attributes.GetNamedItem(IConfigurationStorage.DocGuidField)?.Value is not null
                        && xmlRoot.Attributes.GetNamedItem(IConfigurationStorage.DocStatusField)?.Value is not null)
                    {
                        if (xmlRoot.HasChildNodes)
                        {
                            foreach (XmlNode linkNode in xmlRoot.ChildNodes)
                            {
                                if (linkNode.Name == IConfigurationStorage.LinkNodeName
                                    && linkNode.Attributes.GetNamedItem(IConfigurationStorage.LinkNameField)?.Value is not null
                                    && linkNode.Attributes.GetNamedItem(IConfigurationStorage.LinkGuidField)?.Value is not null)
                                {
                                    return true;
                                }
                                xmlDoc = null;
                                return false;
                            }
                        }
                        return true;
                    }
                    xmlDoc = null;
                    return false;
                }
                catch (XmlException)
                {
                    //TaskDialog.Show("Error", "Configuration file is corrupted or invalid.");
                }
            }
            xmlDoc = null;
            return false; // Invalid configuration file

            bool ValidateMetaInfo(string confFilePath)
            {
                string? firstLine;
                using (var reader = new StreamReader(confFilePath))
                {
                    firstLine = reader.ReadLine();
                }

                if (firstLine is not null && firstLine.StartsWith("<?xml"))
                {
                    // Парсим вручную
                    var match = System.Text.RegularExpressions.Regex.Match(firstLine,
                        @"<\?xml\s+version\s*=\s*""(?<version>[^""]+)""\s*encoding\s*=\s*""(?<encoding>[^""]+)""\s*(standalone\s*=\s*""(?<standalone>[^""]+)"")?\s*\?>");

                    if (match.Success)
                    {
                        string version = match.Groups["version"].Value;
                        string encoding = match.Groups["encoding"].Value;
                        string standalone = match.Groups["standalone"].Value;

                        if (version == "1.0" && encoding == "utf-8" && standalone == "yes") return true;
                    }
                }
                return false; // Если не удалось прочитать или разобрать первую строку
            }
        }

        public DocumentModelInWPF? DeserializeConfig(Guid projectGuid)
        {
            string? projectName = null;
            bool status = false;
            List<LinkModelInWPF>? linkList = new();


            PrepareConfigFile();
            if (ConfigurationDocument.DocumentElement is XmlNode confProjRoot && confProjRoot?.Attributes is not null)
            {
                if (confProjRoot.Attributes.GetNamedItem(IConfigurationStorage.DocGuidField) is not XmlNode attr || attr.Value != projectGuid.ToString()) throw new Exception("***");
                // Нужно динамически проверять имя перед отображением
                projectName = confProjRoot.Attributes.GetNamedItem(IConfigurationStorage.DocNameField)?.Value;
                status = confProjRoot.Attributes.GetNamedItem(IConfigurationStorage.DocStatusField)?.Value == "Enable";
                
                if (ConfigurationDocument.DocumentElement.HasChildNodes)
                {
                    foreach (XmlNode linkNode in ConfigurationDocument.DocumentElement.ChildNodes)
                    {
                        linkList.Add(new LinkModelInWPF(linkNode.Attributes.GetNamedItem(IConfigurationStorage.LinkNameField).Value,
                                                        new Guid(linkNode.Attributes.GetNamedItem(IConfigurationStorage.LinkGuidField).Value),
                                                        true)); // True тк нахождение в конфиге являет отслеживание
                    }
                }
            }
            Document = new DocumentModelInWPF(projectName, projectGuid, status) { LinksInDocument = linkList };
            return Document;
        }
        public void SerializeConfig()
        {

            PrepareConfigFile();
            XmlElement projectElement = ConfigurationDocument.DocumentElement;
            projectElement.SetAttribute(IConfigurationStorage.DocNameField, Document.Name);
            projectElement.SetAttribute(IConfigurationStorage.DocGuidField, Document.Guid.ToString());
            projectElement.SetAttribute(IConfigurationStorage.DocStatusField, Document.Status ? "Enable" : "Disable");
            if (Document.LinksInDocument.Count != 0)
            {
                foreach (LinkModelInWPF link in Document.LinksInDocument)
                {
                    if (link.IsMonitoring)
                    {
                        XmlElement linkElement = ConfigurationDocument.CreateElement(IConfigurationStorage.LinkNodeName);
                        linkElement.SetAttribute(IConfigurationStorage.LinkNameField, link.Name);
                        linkElement.SetAttribute(IConfigurationStorage.LinkGuidField, link.Guid.ToString());
                        projectElement.AppendChild(linkElement);
                    }
                }
            }
            ConfigurationDocument.Save(ConfigFilePath);
            return;
        }
    }
}
