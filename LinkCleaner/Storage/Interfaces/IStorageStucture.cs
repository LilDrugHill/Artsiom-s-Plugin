using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkCleaner.Storage.Interfaces
{

    // TODO: Реализовать отдельный конфиг для каждого проекта и создавать экземпляры для каждого проекта
    public interface IStorage
    {
        static readonly string ConfigurationDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LinkCleaner");
        static readonly string PathToConfiguration = System.IO.Path.Combine(ConfigurationDirectory, "MonitoringConfig.xml");
        static readonly string RootName = "Data"; // Root element for the XML configuration file

        static void CreateConfigFile() { }
        static void LoadConfig() { }
        static void SaveConfig() { }
    }
}
