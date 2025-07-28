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

using LinkCleaner.Presentation.Views;
using LinkCleaner.RevitApp.Interfaces;
using LinkCleaner.Storage.Services;
using LinkCleaner.Presentation.Models;
using LinkCleaner.Presentation.Interfaces;

namespace LinkCleaner.RevitApp.Services
{
    [Transaction(TransactionMode.Manual)]
    internal class StartPresentation : IExternalCommand, IButtonData
    {
        public string Name => "Link Cleaner";
        public IDocumentViewData? Document { get; private set; }
        public string IconName => "link_cleaner_icon.png"; // Ensure this icon exists in the specified path
        private MainWindow? _mainWindow;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // This method is called when the command is executed.
            // You can add your command execution code here.

            UIDocument uiDoc = commandData.Application.ActiveUIDocument; // Получение активного документа
            Document doc = uiDoc.Document; // Получение документа

            MonitoringConfig mc = new MonitoringConfig(doc.CreationGUID);
            IDocumentViewData docWPF = mc.Document;
            Document = docWPF;

            if (docWPF.Name is null ) docWPF.Name = doc.Title; // Установка имени документа, если оно не задано 

            if (Utils.Getters.TryGetLinks(doc, out RevitLinkInstance[]? linksInRevit))
            {
                // Массив гуидов 
                foreach (RevitLinkInstance linkRVT in linksInRevit)
                {
                    // Сравнивать гуиды и давать мониторинг ссылке или нет
                    if (docWPF.LinksInDocument.Find(linkWPF => linkWPF.Guid == linkRVT.GetLinkDocument().CreationGUID) is LinkModelInWPF linkWPF)
                    {
                        
                        // обновить имя если оно не совпадает
                        linkWPF.Name = linkRVT.Name.Split(':')[0];
                    } else
                    {
                       docWPF.LinksInDocument.Add(new LinkModelInWPF(linkRVT.Name.Split(':')[0], linkRVT.GetLinkDocument().CreationGUID, false));
                    }
                }
            }

            if (_mainWindow == null || !_mainWindow.IsLoaded)
            {
                _mainWindow = new MainWindow(mc);
                _mainWindow.ShowDialog(); // Показывает окно
            }
            else
            {
                _mainWindow.Activate(); // Уже открыто — активируем
            }

            return Result.Succeeded;

        }



    }


}
