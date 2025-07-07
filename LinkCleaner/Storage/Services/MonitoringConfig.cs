using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;

using LinkCleaner.Presentation.Emuns;
using LinkCleaner.Presentation.Models;
using LinkCleaner.Storage.Constants;

namespace LinkCleaner.Storage.Services
{
    public class MonitoringConfig
    {
        //public string Name => "Add Link";
        //public string IconName => "link_cleaner_icon.png";
        //public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        //{
        //    UIDocument uiDoc = commandData.Application.ActiveUIDocument;
        //    Document doc = uiDoc.Document;

        //    RevitLinkType[] links = App.GetLinks(doc);

        //    if (links.Length == 0)
        //    {
        //        TaskDialog.Show("Info", "No links found in the document.");
        //        return Result.Failed;
        //    }
        XmlDocument ConfigurationDocument;
        
        public string ConfigFilePath { get; init; }
        public string ConfigDirPath { get; init; }

        DocumentModelInWPF _document;

        public DocumentModelInWPF Document { get => _document; init => _document = value; }

        public MonitoringConfig(Guid projectGuid, string? confDir = null)
        {
            ConfigDirPath = confDir ?? ConfigConstants.PathToConfigurationDirectory;
            ConfigFilePath = Path.Combine(ConfigDirPath, $"{projectGuid.ToString()}.xml");

            DeserializeConfig();
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
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Data"));
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
                    if (root is XmlElement xmlRoot && xmlRoot.Name == ConfigConstants.RootName)
                    {
                        if (xmlRoot.HasChildNodes)
                        {
                            foreach (XmlNode projectNode in xmlRoot.ChildNodes)
                            {
                                if (projectNode.Name == ConfigConstants.ProjectNodeName
                                    && projectNode.Attributes.GetNamedItem(ConfigConstants.DocNameField)?.Value is not null
                                    && projectNode.Attributes.GetNamedItem(ConfigConstants.DocGuidField)?.Value is not null
                                    && projectNode.Attributes.GetNamedItem(ConfigConstants.DocStatusField)?.Value is not null)
                                {
                                    if (projectNode.HasChildNodes)
                                    {
                                        foreach (XmlNode linkNode in projectNode.ChildNodes)
                                        {
                                            if (linkNode.Name == ConfigConstants.LinkNodeName
                                                && linkNode.Attributes.GetNamedItem(ConfigConstants.LinkNameField)?.Value is not null
                                                && linkNode.Attributes.GetNamedItem(ConfigConstants.LinkGuidField)?.Value is not null)
                                            {
                                                return true;
                                            }
                                            xmlDoc = null;
                                            return false;
                                        }
                                        return true;
                                    }
                                    return true;
                                }
                                xmlDoc = null;
                                return false;
                            }
                            return true;
                        }
                        return true; // Valid configuration file
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

        public List<DocumentModelInWPF> DeserializeConfig()
        {
            PrepareConfigFile();
            if (ConfigurationDocument.DocumentElement.HasChildNodes && ConfigurationDocument.DocumentElement.ChildNodes is XmlNodeList Nodes)
            {
                foreach (XmlNode project in Nodes)
                {
                    string projectName = project.Attributes.GetNamedItem(ConfigConstants.DocNameField).Value;
                    Guid projectGuid = new Guid(project.Attributes.GetNamedItem(ConfigConstants.DocGuidField).Value);
                    bool status = project.Attributes.GetNamedItem(ConfigConstants.DocStatusField).Value == "Enable";
                    List<LinkModelInWPF> links = new();
                    if (project.HasChildNodes)
                    {
                        foreach (XmlNode linksInConfig in project.ChildNodes)
                        {
                            links.Add(new LinkModelInWPF(linksInConfig.Attributes.GetNamedItem(ConfigConstants.LinkNameField).Value,
                                                            new Guid(linksInConfig.Attributes.GetNamedItem(ConfigConstants.LinkGuidField).Value),
                                                            true));
                        }
                    }
                    Document = new DocumentModelInWPF(projectName, projectGuid, status) { LinksInDocument = links };
                }
            }
            return DocumentsList;

        }

        //static bool IsProjectInConfig(Guid projectGuid)
        //{
        //    return ConfigDict.ContainsKey(projectGuid);
        //}

        //static void AddProjectToConfig(Guid projectGuid)
        //{
        //    if (!ConfigDict.ContainsKey(projectGuid))
        //    {
        //        ConfigDict[projectGuid] = new List<Guid>();
        //    }
        //}

        //static void AddLinkToConfig(Guid projectGuid, Guid linkGuid)
        //{
        //    if (!ConfigDict.ContainsKey(projectGuid))
        //    {
        //        ConfigDict[projectGuid] = new List<Guid>();
        //    }
        //    ConfigDict[projectGuid].Add(linkGuid);
        //}

        //static void RemoveLinkFromConfig(Guid projectGuid, Guid linkGuid)
        //{
        //    if (ConfigDict.ContainsKey(projectGuid))
        //    {
        //        ConfigDict[projectGuid].Remove(linkGuid);
        //    }
        //}

        //static void RemoveProjectFromConfig(Guid projectGuid)
        //{
        //    if (ConfigDict.ContainsKey(projectGuid))
        //    {
        //        ConfigDict.Remove(projectGuid);
        //    }
        //}
        public void SerializeConfig(string confFile)
        {

            PrepareConfigFile();

            if (DocumentsList.Count != 0)
            {
                foreach (DocumentModelInWPF documentModelInWPF in DocumentsList)
                {
                    XmlElement projectElement = ConfigurationDocument.CreateElement(ConfigConstants.ProjectNodeName);
                    projectElement.SetAttribute(ConfigConstants.DocNameField, documentModelInWPF.Name);
                    projectElement.SetAttribute(ConfigConstants.DocGuidField, documentModelInWPF.Guid.ToString());
                    projectElement.SetAttribute(ConfigConstants.DocStatusField, documentModelInWPF.Status ? "Enable" : "Disable");
                    if (documentModelInWPF.LinksInDocument.Count != 0)
                    {
                        foreach (LinkModelInWPF link in documentModelInWPF.LinksInDocument)
                        {
                            if (link.IsMonitoring)
                            {
                                XmlElement linkElement = ConfigurationDocument.CreateElement(ConfigConstants.LinkNodeName);
                                linkElement.SetAttribute(ConfigConstants.LinkNameField, link.Name);
                                linkElement.SetAttribute(ConfigConstants.LinkGuidField, link.Guid.ToString());
                                projectElement.AppendChild(linkElement);
                            }
                        }
                    }
                    if (ConfigurationDocument?.DocumentElement?.AppendChild(projectElement) is null) throw new FileLoadException("Save exeption");
                }
            }
            ConfigurationDocument.Save(confFile);
            return;





        //    return Result.Succeeded;
        //}

        }

        void ClearCache()
        {
            DocumentsList.Clear();
            ConfigurationDocument = null;
        }

    }
}
