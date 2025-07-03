using System.Windows;

using Autodesk.Revit.DB;

using LinkCleaner.Presentation.ViewModels;

namespace LinkCleaner.Presentation.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(Document doc)
        {
            InitializeComponent();
            var vm = new MainViewModel();
            vm.RequestClose += (s, e) => this.Close(); // Подписка на событие закрытия окна
            
            DataContext = vm; // Установка контекста данных для привязки
        }



    }
}
