using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using LinkCleaner.Presentation.Models;

namespace LinkCleaner.RevitApp.Utils
{
    public static class CovertToLinkModelInWPF
    {
        // This class is intended to convert Revit document models to WPF-friendly models.
        // It will handle the conversion logic and provide methods to access the data in a WPF-compatible format.
        // Example method to convert a Revit document to a WPF model
        public static LinkModelInWPF ConvertDocumentToWpfModel(this RevitLinkInstance link, bool isMonitoring)
        {
            // Conversion logic goes here
            // This is a placeholder implementation
            return new LinkModelInWPF(link.Name, link.VersionGuid, isMonitoring);
                // Set additional properties if needed
        }

        // Additional methods for conversion can be added here
    }
}
