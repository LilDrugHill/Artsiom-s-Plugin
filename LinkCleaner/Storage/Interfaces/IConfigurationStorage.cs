using LinkCleaner.Presentation.Interfaces;
using LinkCleaner.Presentation.Models;
using System.IO;


namespace LinkCleaner.Storage.Interfaces
{
    public interface IConfigurationStorage
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

        IDocumentViewData? Document {  get; }

        public void SerializeConfig();
        public void DeserializeConfig(Guid projectGuid);
        
    }
}
