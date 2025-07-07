using System.IO;
using LinkCleaner.Presentation.Models;


namespace LinkCleaner.Storage.Interfaces
{
    interface IConfigurationStorage
    {
        public static readonly string PathToConfigurationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LinkCleaner");

        public const string DocNameField = "Name"; // Name of the document
        public const string DocGuidField = "Guid";
        public const string DocStatusField = "Status"; // Status of the document (e.g., Active, Inactive)
        public const string LinkNameField = "Name"; // Name of the link
        public const string LinkGuidField = "Guid"; // Unique identifier for the link
        public const string LinkIsMonitoringField = "IsMonitoring"; // Indicates if the link is being monitored

        public const string RootName = "Project";
        public const string LinkNodeName = "Link";

        DocumentModelInWPF? Document {  get; }
        

    }
}
