using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Autodesk.Revit.DB;

using LinkCleaner.Presentation.Models;
using LinkCleaner.Presentation.Commands;
using LinkCleaner.RevitApp.Services;
using LinkCleaner.Storage.Services;

namespace LinkCleaner.Presentation.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        Dictionary<Guid, List<Guid>> configDict1 = new Dictionary<Guid, List<Guid>>();
        DocumentModelInWPF _document;
        public event PropertyChangedEventHandler? PropertyChanged;
        public DocumentModelInWPF Document
        {
            get { return _document; }
            set { _document = value; }
        }
        public MainViewModel()
        {

            Document = new DocumentModelInWPF("testDoc", new Guid("a87274e2-4956-4344-8b98-e23b5f0733d5"), true) { LinksInDocument = new List<LinkModelInWPF>() };
            Document.LinksInDocument.Add(new LinkModelInWPF("testLink1", new Guid("a87274e2-4956-4344-8b98-e23b5f0733d5"), true));
            Document.LinksInDocument.Add(new LinkModelInWPF("testLink2", new Guid("a87274e2-4956-4344-8b98-e23b5f0733d5"), false));

            CloseCommand = new RelayCommand(() => RequestClose?.Invoke(this, EventArgs.Empty));
            ApplyChangesToConfigCom = new RelayCommand(() => ApplyChangesToConfigEv?.Invoke(this, EventArgs.Empty));

        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand CloseCommand { get; }
        public ICommand ApplyChangesToConfigCom { get; }
        public event EventHandler RequestClose;
        public event EventHandler ApplyChangesToConfigEv;


    }
}
