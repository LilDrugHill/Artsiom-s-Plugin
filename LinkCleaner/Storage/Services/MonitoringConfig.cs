using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;

using LinkCleaner.Presentation.Emuns;
using LinkCleaner.Presentation.Models;
using LinkCleaner.Storage.Constants;

namespace LinkCleaner.Storage.Services
{
    public static partial class MonitoringConfig
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
        static XmlDocument ConfigurationDocument;
        static bool _hasProjects = false;
        static bool _hasLinks = false;

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
            string configurationDirectory = confDir ?? ConfigConstants.PathToConfigurationDirectory;
            string pathToConfigurationFile = confFile ?? ConfigConstants.PathToConfigurationFile;

            try
            {
                switch (Directory.Exists(configurationDirectory), File.Exists(pathToConfigurationFile))
                {
                    case ( false, false):
                        Directory.CreateDirectory(configurationDirectory);
                        CreateConfFile(pathToConfigurationFile);
                        break;
                    case ( true, false):
                        CreateConfFile(pathToConfigurationFile);
                        break;
                    case ( true, true):
                        if (!TryGetValidXmlConfig(pathToConfigurationFile, out XmlDocument? xmlDoc))
                        {
                            File.Delete(pathToConfigurationFile);
                            CreateConfFile(pathToConfigurationFile);
                        }
                        ConfigurationDocument = xmlDoc;
                        break;
                }
            }
            catch (Exception ex)
            {
                //TaskDialog.Show("Error", $"Failed to create config file: {ex.Message}.");
            }


        }
        static void CreateConfFile(string path)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Data"));
            doc.Save(path);
        }

        //
        /// <summary>
        /// If it returns true. out set loaded XML configuration. If false out is null.
        /// </summary>
        /// <param name="confFile"></param>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public static bool TryGetValidXmlConfig(string confFile, out XmlDocument? xmlDoc) 
        {

            if (File.Exists(confFile))
            {
                xmlDoc = new();

                try
                {
                    xmlDoc.Load(confFile);
                    XmlNode root = ConfigurationDocument.DocumentElement;
                    if (root is XmlElement xmlRoot && xmlRoot.Name == ConfigConstants.RootName)
                    {
                        if (xmlRoot.HasChildNodes)
                        {
                            foreach (XmlNode projectNode in xmlRoot.ChildNodes)
                            {
                                if(projectNode.Name == ConfigConstants.ProjectNodeName 
                                    && projectNode.Attributes.GetNamedItem(ConfigConstants.DocNameField)?.Value is not null
                                    && projectNode.Attributes.GetNamedItem(ConfigConstants.DocGuidField)?.Value is not null
                                    && projectNode.Attributes.GetNamedItem(ConfigConstants.DocStatusField)?.Value is not null)
                                {
                                    if (projectNode.HasChildNodes)
                                    {
                                        foreach (XmlNode linkNode in projectNode.ChildNodes)
                                        {
                                            if (linkNode.Name == ConfigConstants.LinkNodeName
                                                && linkNode.Attributes.GetNamedItem(ConfigConstants.LinkNodeName)?.Value is not null
                                                && linkNode.Attributes.GetNamedItem(ConfigConstants.LinkGuidField)?.Value is not null
                                                && linkNode.Attributes.GetNamedItem(ConfigConstants.LinkIsMonitoringField)?.Value is not null) 
                                            { }
                                            xmlDoc = null;
                                            return false;
                                        }
                                        _hasLinks
                                        return true
                                    }
                                    return true;

                                }
                                xmlDoc = null;
                                return false;
                            }
                            _hasProjects = true;
                            return true
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
        }

        static void LoadConfig()
        {
            if (ConfigurationDocument is null) throw new FileLoadException("Conf doesn't exists");

            if (ConfigurationDocument.DocumentElement is XmlElement XmlRoot && XmlRoot.Name == ConfigConstants.RootName)
            {
                if (XmlRoot.HasChildNodes)
                {
                    foreach (XmlNode project in XmlRoot.ChildNodes)
                    {
                        string projectName = project.Attributes.GetNamedItem(ConfigConstants.DocNameField).Value;
                        Guid projectGuid = new Guid(project.Attributes.GetNamedItem(ConfigConstants.DocGuidField).Value);
                        Enum.TryParse(project.Attributes.GetNamedItem(ConfigConstants.DocStatusField).Value, out Status status);
                        if (project.HasChildNodes)
                        {
                            LinkModelInWPF[] links = new LinkModelInWPF[project.ChildNodes.Count];
                            int i = 0;
                            foreach (XmlNode linksInConfig in project.ChildNodes)
                            {
                                links[i] = new LinkModelInWPF(linksInConfig.Attributes.GetNamedItem(ConfigConstants.LinkNameField).Value, 
                                                              new Guid(linksInConfig.Attributes.GetNamedItem(ConfigConstants.Lin).Value), 
                                                              true);
                                i++;
                            }
                        }
                    }

                }
            }
            ConfigurationDocument = null;
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
            PrepareConfigFile();
            if (DocumentsList.Count != 0)
            {
                foreach (DocumentModelInWPF documentModelInWPF in DocumentsList)
                {
                    XmlElement projectElement = ConfigurationDocument.CreateElement(ConfigConstants.ProjectNodeName);
                    projectElement.SetAttribute(ConfigConstants.DocNameField, documentModelInWPF.Name);S
                    projectElement.SetAttribute(ConfigConstants.DocGuidField, documentModelInWPF.Guid.ToString());
                    projectElement.SetAttribute(ConfigConstants.DocStatusField, documentModelInWPF.Status.ToString());
                    if (documentModelInWPF?.LinksInDocument?.Length != 0)
                    {
                        foreach (LinkModelInWPF link in documentModelInWPF.LinksInDocument)
                        {
                            XmlElement linkElement = ConfigurationDocument.CreateElement(ConfigConstants.LinkNodeName);
                            linkElement.SetAttribute(ConfigConstants.LinkNameField, link.Name);
                            linkElement.SetAttribute(ConfigConstants.LinkGuidField, link.Guid.ToString());
                            linkElement.SetAttribute(ConfigConstants.LinkIsMonitoringField, link.IsMonitoring ? "1" : "0");
                            projectElement.AppendChild(linkElement);
                        }
                    }
                    if (ConfigurationDocument?.DocumentElement?.AppendChild(projectElement) is null) throw new FileLoadException("Save exeption");
                }
            }
            ConfigurationDocument.Save(ConfigConstants.PathToConfigurationFile);
            return;





        //    return Result.Succeeded;
        //}

        }

    }
}
