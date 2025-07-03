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
    }
}
