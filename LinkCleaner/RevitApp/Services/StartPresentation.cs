using Autodesk.Revit;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLinq;
using Autodesk.Revit.UI;

using LinkCleaner.Presentation.ViewModels;
using LinkCleaner.Presentation.Views;
using LinkCleaner.RevitApp.Interfaces;
using LinkCleaner.Storage.Services;
using LinkCleaner.Presentation.Models;

namespace LinkCleaner.RevitApp.Services
{
    [Transaction(TransactionMode.Manual)]
    internal class StartPresentation : IExternalCommand, IButtonData
    {
        public string Name => "Link Cleaner";
        public string IconName => "link_cleaner_icon.png"; // Ensure this icon exists in the specified path
        private MainWindow? _mainWindow;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // This method is called when the command is executed.
            // You can add your command execution code here.

            UIDocument uiDoc = commandData.Application.ActiveUIDocument; // Получение активного документа
            Document doc = uiDoc.Document; // Получение документа

            MonitoringConfig mc = new MonitoringConfig(doc.CreationGUID);
            DocumentModelInWPF docWPF = mc.Document;

            if (docWPF.Name is null ) docWPF.Name = doc.Title; // Установка имени документа, если оно не задано 

            if (TryGetLinks(doc, out RevitLinkType[]? linksInRevit))
            {
                // Массив гуидов 
                foreach (RevitLinkType linkIR in linksInRevit)
                {
                    // Сравнивать гуиды и давать мониторинг ссылке или нет
                    bool monStatus = false;
                    if (docWPF.LinksInDocument.Find(x => x.Guid.ToString() == l))
                    {
                        docWPF.LinksInDocument.Find
                        monStatus = true;
                        // обновить имя если оно не совпадает а гуид совпадает
                    }

                    docWPF.LinksInDocument.Add(new LinkModelInWPF(lind.Name, lind.VersionGuid, monStatus));

                }
            }


            if (_mainWindow == null || !_mainWindow.IsVisible)
            {
                _mainWindow = new MainWindow(doc);
                _mainWindow.Show(); // Показывает окно
            }
            else
            {
                _mainWindow.Activate(); // Уже открыто — активируем
            }

            return Result.Succeeded;

        }
        public static bool TryGetLinks(Document doc, out RevitLinkType[] LinksInDocument)
        {
            LinksInDocument = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkType))
                .AsValueEnumerable()
                .Cast<RevitLinkType>()
                .Where(link => !link.IsNestedLink)
                .ToArray();
            return LinksInDocument.Length > 0 ? true : false;
        }
    }


}
