using System.IO;


namespace LinkCleaner.Storage.Constants
{
    public static class ConfigConstants
    {
        public static readonly string PathToConfigurationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LinkCleaner");
        public static readonly string PathToConfigurationFile = Path.Combine(PathToConfigurationDirectory, "MonitoringConfig.xml");

        public const string DocNameField = "Name"; // Name of the document
        public const string DocGuidField = "Guid";
        public const string DocStatusField = "Status"; // Status of the document (e.g., Active, Inactive)
        public const string LinkNameField = "Name"; // Name of the link
        public const string LinkGuidField = "Guid"; // Unique identifier for the link
        public const string LinkIsMonitoringField = "IsMonitoring"; // Indicates if the link is being monitored

        public const string RootName = "Data"; // Root element for the XML configuration file
        public const string ProjectNodeName = "Project";
        public const string LinkNodeName = "Link";


    }
}
