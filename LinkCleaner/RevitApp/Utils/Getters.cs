using Autodesk.Revit.DB;

using ZLinq;

namespace LinkCleaner.RevitApp.Utils
{
    public static class Getters
    {
        public static bool TryGetLinks(Document doc, out RevitLinkInstance[] LinksInDocument)
        {
            LinksInDocument = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkInstance))
                .AsValueEnumerable()
                .Cast<RevitLinkInstance>()
                .Where(link => link?.GetLinkDocument() is not null)
                .ToArray();
            return LinksInDocument.Length > 0 ? true : false;
        }
    }
}
