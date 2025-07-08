//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Autodesk.Revit.Attributes;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;

//using ZLinq;

//using LinkCleaner.RevitApp.Services;
//using LinkCleaner.RevitApp.Interfaces;
//using LinkCleaner.Presentation.Models;

//namespace LinkCleaner.RevitApp.Utils
//{
//    [Transaction(TransactionMode.Manual)]
//    internal class StartPresentation : IExternalCommand, IButtonData
//    {
//        public string Name => "LinkCleaner"; // Название кнопки
//        public string IconName => "link_cleaner_icon.png"; // Путь к иконке кнопки (убедитесь, что иконка существует в указанном пути)
//                                                           // Метод, который будет выполнен при нажатии на кнопку

//        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//        {
//            UIDocument uiDoc = commandData.Application.ActiveUIDocument; // Получение активного документа  
//            Document doc = uiDoc.Document; // Получение документа

          
//            ;

//            // Получение всех линков в документе

//            //Приведение к GUID для дальнейшей работы с конфигурацией

//            // Данные для вьюхи

//            // Получение отслеживаемых ссылок из конфигурации для данного документа(проекта)
//            // Создание обьектов модели Link из массивов. Пересечения будут выделены другим цветом
//            //if (MonitoringConfig.ConfigDict.TryGetValue(doc.CreationGUID, out List<Guid>? MonitoringLinksGuids) 
//            //    && MonitoringLinksGuids.Count > 0)
//            //{
//            //    foreach (RevitLinkType linkType in AllDocumentsLinksTypes)
//            //    {
//            //        linksGuids.Add(new LinkModelInWPF(linkType.Name, linkType.VersionGuid, true));
//            //        //{
//                    //    ClickCommand = new DelegateCommand(() =>
//                    //    {
//                    //        // Здесь можно добавить логику для обработки клика по ссылке
//                    //        // Например, можно выделить ссылку в интерфейсе или показать дополнительную информацию
//                    //    }),
//                    //    //IsMonitoring = MonitoringLinksGuids.Contains(linkType.VersionGuid) // Отслеживаемая ли ссылка
//                    //});
//                //}
//            //}


//            //MainWindow mainWindow = new MainWindow(linksGuids);

//            //DeleteLinks(doc, rl); // Удаление ссылок
//            return Result.Succeeded;
//        }
//        void DeleteLinks(Document doc, RevitLinkType[] links)
//        {
//            if (links.Length == 0)
//            {
//                return;
//            }
//            using (Transaction tx = new Transaction(doc, "Delete Links"))
//            {
//                tx.Start();

//                FailureHandlingOptions failOpts = tx.GetFailureHandlingOptions();
//                failOpts.SetFailuresPreprocessor(new WarningSuppressor());
//                tx.SetFailureHandlingOptions(failOpts);

//                foreach (RevitLinkType link in links)
//                {
//                    doc.Delete(link.Id); // Удаление всех связанных файлов
//                }

//                tx.Commit();
//            }
//        }
//    }
//}
