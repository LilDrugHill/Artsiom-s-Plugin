using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ZLinq;

using LinkCleaner.RevitApp.Interfaces;

namespace LinkCleaner.RevitApp.Services
{
    public class App : IExternalApplication
    {
        public static readonly string GlobalParameterName = "LinkCleanerGlobalParameter"; // Имя глобального параметра, который будет создан при запуске приложения
        public static GlobalParameter GlobalParameter
        {
            get => _globalParameter;
            set
            {
                _globalParameter = value;
            }
        }
        static GlobalParameter _globalParameter = null; // Переменная для хранения глобального параметра



        public Result OnStartup(UIControlledApplication application)
        {
            // This method is called when the Revit application starts up.
            // You can add your initialization code here.

            string tabName = "LinkCleaner";

            application.CreateRibbonTab(tabName);
            RibbonPanel ribbonPanel = application.CreateRibbonPanel(tabName, "LinkCleaner");

            ribbonPanel.AddItem(CreateButton<StartPresentation>(assemblyLocation));

            //application.ControlledApplication.DocumentSynchronizingWithCentral += OnDocumentSycing;
            //application.ControlledApplication.DocumentSynchronizedWithCentral += OnDocumentSyced;
            //application.ControlledApplication.DocumentChanged 


            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            // This method is called when the Revit application shuts down.
            // You can add your cleanup code here.
            return Result.Succeeded;
        }

        //private void OnDocumentSycing(object sender, DocumentSynchronizingWithCentralEventArgs e)
        //{
        //    // This method is called when a document is closing.
        //    // You can add your cleanup code here.
        //    if (GlobalParameter != null)
        //    {
        //        LinksDeletor(e.Document, LinksCache.SetCache(e.Document, GetLinks(e.Document)));
        //    }
        //}

        //private void OnDocumentSyced(object sender, DocumentSynchronizedWithCentralEventArgs e)
        //{
        //    if (GlobalParameter != null && LinksCache.TryGetChachedLinks(e.Document, out RevitLinkType[] rl)) 
        //    {
        //        ReturnLinks(e.Document, rl);
        //    }
        //}
        // На случай аснихронного кода
        //private void AddLinkToStorage(object sender, DocumentChangedEventArgs e)
        //{
        //    Document doc = e.GetDocument();
        //    var AddedElementIds = e.GetAddedElementIds();
        //    foreach (var id in AddedElementIds)
        //    {
        //        var element = doc.GetElement(id);
        //        if (element is RevitLinkInstance linkInstance)
        //        {

        //        }
        //    }
        //}

        static string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        public static readonly string AppsDirectoryPath = Path.GetDirectoryName(assemblyLocation);
        /// Часть необходимая для создания и обращения к хранилищу иконок конпок

        static PushButtonData CreateButton<T>(string assemblyLocation) where T : IExternalCommand, IButtonData, new()
        {
            T commandInstatce = new T();
            return new PushButtonData(typeof(T).Name,
                                      commandInstatce.Name,
                                      assemblyLocation,
                                      typeof(T).FullName)
            {
                LargeImage = new BitmapImage(new Uri(AppsDirectoryPath + @"\icons\" + commandInstatce.IconName))
            };
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
