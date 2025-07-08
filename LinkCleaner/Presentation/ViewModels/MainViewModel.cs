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

namespace LinkCleaner.Presentation.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        Dictionary<Guid, List<Guid>> configDict1 = new Dictionary<Guid, List<Guid>>();
        List<LinkModelInWPF> _links;
        public event PropertyChangedEventHandler? PropertyChanged;
        public List<LinkModelInWPF> Links
        {
            get { return _links; }
            set
            {
                _links = value;
                OnPropertyChanged(nameof(Links));
            }
        }
        public MainViewModel()
        {
            //if (App.TryGetLinks(doc, out RevitLinkType[] linksInDocument))
            //{
            // Из CreationGuid
            //}
            ;
            List<LinkModelInWPF> RevitDocumentGuids = new List<LinkModelInWPF>()
            {
                new LinkModelInWPF("Link1", Guid.NewGuid(), true),
                new LinkModelInWPF("Link2", Guid.NewGuid(), false),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true),
                new LinkModelInWPF("Link3", Guid.NewGuid(), true)

            };
            Links = RevitDocumentGuids;
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
