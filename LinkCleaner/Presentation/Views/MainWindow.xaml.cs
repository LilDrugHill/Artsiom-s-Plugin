using System.Windows;

using Autodesk.Revit.DB;

using LinkCleaner.Presentation.ViewModels;
using LinkCleaner.Storage.Services;
using LinkCleaner.Storage.Interfaces;

namespace LinkCleaner.Presentation.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IConfigurationStorage mc)
        {

            InitializeComponent();
            var vm = new MainViewModel();
            vm.RequestClose += (s, e) =>
            {
                this.Close(); // Подписка на событие закрытия окна
            };
            vm.Document = mc.Document ?? throw new System.Exception("Configuration document is null"); // Установка документа в модель представления
            //Task task = new Task(() => mc.SerializeConfig());
            vm.OnApplyChangesToConfig += (s, e) => {mc.SerializeConfig(); this.Close(); };
            vm.OnToggleDocumentStatus += (s, e) => mc.Document.Status = !mc.Document.Status;
            DataContext = vm; // Установка контекста данных для привязки
        }



    }
}
