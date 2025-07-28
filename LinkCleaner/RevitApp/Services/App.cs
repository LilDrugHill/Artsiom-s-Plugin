using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using LinkCleaner.RevitApp.Interfaces;
using LinkCleaner.RevitApp.Utils;
using LinkCleaner.Storage.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZLinq;

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

            application.ControlledApplication.DocumentSynchronizingWithCentral += OnDocumentSycing;



            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            // This method is called when the Revit application shuts down.
            // You can add your cleanup code here.
            return Result.Succeeded;
        }

        private void OnDocumentSycing(object sender, DocumentSynchronizingWithCentralEventArgs e)
        {
            // This method is called when a document is closing.
            // You can add your cleanup code here.
            MonitoringConfig mc = new MonitoringConfig(e.Document.CreationGUID);
            if (!mc.Document.Status) return;
            Getters.TryGetLinks(e.Document, out RevitLinkInstance[]? linksInRevit);
            // Подготовим HashSet<Guid> для быстрого поиска
            var monitoredGuids = mc.Document.LinksInDocument
                .Select(link => link.Guid)
                .ToHashSet();

            // Фильтруем ссылки
            var links = linksInRevit
                .Where(linkRVT =>
                {
                    var linkedDoc = linkRVT.GetLinkDocument();
                    return linkedDoc != null && monitoredGuids.Contains(linkedDoc.CreationGUID);
                });

            using (Transaction tx = new Transaction(e.Document, "Delete Links"))
            {
                tx.Start();

                FailureHandlingOptions failOpts = tx.GetFailureHandlingOptions();
                failOpts.SetFailuresPreprocessor(new WarningSuppressor());
                tx.SetFailureHandlingOptions(failOpts);

                foreach (RevitLinkInstance linkI in links)
                {
                    RevitLinkType linkT = e.Document.GetElement(linkI.GetTypeId()) as RevitLinkType;
                    e.Document.Delete(linkT.Id); // Удаление всех связанных файлов
                }

                tx.Commit();
            }
        }

        //private void OnDocumentSyced(object sender, DocumentSynchronizedWithCentralEventArgs e)
        //{
        //    if (GlobalParameter != null && LinksCache.TryGetChachedLinks(e.Document, out RevitLinkType[] rl)) 
        //    {
        //        ReturnLinks(e.Document, rl);
        //    }
        //}
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
        public static readonly string AppsDirectoryPath = System.IO.Path.GetDirectoryName(assemblyLocation);
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


    }
}
