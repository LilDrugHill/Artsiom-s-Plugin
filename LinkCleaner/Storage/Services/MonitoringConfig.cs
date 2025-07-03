using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;

using LinkCleaner.Presentation.Emuns;
using LinkCleaner.Presentation.Models;

namespace LinkCleaner.Storage.Services
{
    public static class MonitoringConfig
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
        static readonly string ConfigurationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LinkCleaner");
        static readonly string PathToConfiguration = Path.Combine(ConfigurationDirectory, "MonitoringConfig.xml");
        static readonly string RootName = "Data"; // Root element for the XML configuration file
        static XmlDocument ConfigurationDocument = new XmlDocument();

        static private string _docNameField = "Name"; // Name of the document
        static private string _docGuidField = "Guid";
        static private string _docStatusField = "Status"; // Status of the document (e.g., Active, Inactive)
        static private string _linkNameField = "Name"; // Name of the link
        static private string _linkGuidField = "Guid"; // Unique identifier for the link
        static private string _linkIsMonitoringField = "IsMonitoring"; // Indicates if the link is being monitored

        static public List<DocumentModelInWPF> DocumentsList = new List<DocumentModelInWPF>();

        static MonitoringConfig()
        {
            
        }

        /// <summary>
        /// Создает конфигурационный файл если это требуется. Параметры нужны ТОЛЬКО для тестирования 
        /// </summary>
        /// <param name="confDir"></param>
        /// <param name="confFile"></param>
        public static void PrepareConfigFile(string? confDir = null, string? confFile = null)
        {
            string configurationDirectory = confDir ?? ConfigurationDirectory;
            string pathToConfiguration = confFile ?? PathToConfiguration;

            try
            {
                switch (Directory.Exists(configurationDirectory), File.Exists(pathToConfiguration))
                {
                    case ( false, false):
                        Directory.CreateDirectory(configurationDirectory);
                        CreateConfFile(pathToConfiguration);
                        break;
                    case ( true, false):
                        CreateConfFile(pathToConfiguration);
                        break;
                    case ( true, true):
                        if (!ValidateConfigFile(pathToConfiguration))
                        {
                            File.Delete(pathToConfiguration);
                            CreateConfFile(pathToConfiguration);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                //TaskDialog.Show("Error", $"Failed to create config file: {ex.Message}.");
            }

            static void CreateConfFile(string path)
            {
                var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Data"));
                doc.Save(path);
            }
        }


        static bool ValidateConfigFile(string confFile) 
        {

            if (File.Exists(confFile))
            {
                XmlDocument confDoc = new();

                try
                {
                    confDoc.Load(confFile);
                    XmlNode root = ConfigurationDocument.DocumentElement;
                    if (root is XmlElement xmlRoot && xmlRoot.Name == RootName)
                    {
                        return true; // Valid configuration file
                    }
                }
                catch (XmlException)
                {
                    //TaskDialog.Show("Error", "Configuration file is corrupted or invalid.");
                }
            }
            return false; // Invalid configuration file
        }

        static void LoadConfig()
        {
            ConfigurationDocument.Load(PathToConfiguration);

            XmlNode root = ConfigurationDocument.DocumentElement;
            if (ConfigurationDocument.DocumentElement is XmlElement XmlRoot && root.Name == RootName && root.HasChildNodes)
            {
                foreach (XmlNode project in XmlRoot.ChildNodes)
                {
                    string projectName = project.Attributes.GetNamedItem(_docNameField)?.Value;
                    Guid projectGuid = new Guid(project.Attributes.GetNamedItem(_docGuidField).Value);
                    Enum.TryParse(project.Attributes.GetNamedItem(_docStatusField)?.Value, out Status status);
                    if (project.HasChildNodes)
                    {
                        LinkModelInWPF[] links = new LinkModelInWPF[project.ChildNodes.Count];
                        int i = 0;
                        foreach (XmlNode linksInConfig in project.ChildNodes)
                        {
                            links[i] = new LinkModelInWPF(linksInConfig.Attributes.GetNamedItem(_linkNameField).Value, 
                                                          new Guid(linksInConfig.Attributes.GetNamedItem(_linkGuidField).Value), 
                                                          true);
                        }
                    }
                }
            }
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
        static void SaveConfig()
        {
            if (File.Exists(PathToConfiguration) && ConfigurationDocument.DocumentElement is XmlElement xRoot && xRoot.Name == RootName)
            {
                if (DocumentsList.Count != 0)
                {
                    foreach (DocumentModelInWPF documentModelInWPF in DocumentsList)
                    {
                        XmlElement projectElement = ConfigurationDocument.CreateElement("Project");
                        projectElement.SetAttribute(_docGuidField, documentModelInWPF.Guid.ToString());
                        projectElement.SetAttribute(_docStatusField, documentModelInWPF.Status.ToString());
                        if (documentModelInWPF?.LinksInDocument?.Length != 0)
                        {
                            foreach (LinkModelInWPF link in documentModelInWPF.LinksInDocument)
                            {
                                XmlElement linkElement = ConfigurationDocument.CreateElement("Link");
                                linkElement.SetAttribute(_linkNameField, link.Name);
                                linkElement.SetAttribute(_linkGuidField, link.Guid.ToString());
                                linkElement.SetAttribute(_linkIsMonitoringField, link.IsMonitoring ? "1" : "0");
                                projectElement.AppendChild(linkElement);
                            }
                        }
                        xRoot.AppendChild(projectElement);
                    }
                }
                xRoot.RemoveAll(); // Clear existing nodes before saving new ones
                ConfigurationDocument.Save(PathToConfiguration);
                return;
            }
            else
            {
                //TaskDialog.Show("Error", "Configuration document is not initialized properly.");
            }




            //    return Result.Succeeded;
            //}

        }

    }
}
