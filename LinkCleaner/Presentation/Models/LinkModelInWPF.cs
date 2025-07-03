using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

using LinkCleaner.Presentation.Commands;

namespace LinkCleaner.Presentation.Models
{
    public class LinkModelInWPF : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        string _name;
        Guid _guid;
        bool _isMonitoring;
        public ICommand ClickCommand { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public bool IsMonitoring
        {
            get { return _isMonitoring; }
            set
            {
                _isMonitoring = value;
                OnPropertyChanged(nameof(IsMonitoring));
            }
        }

        public string Name
        {
            get { return _name; }
            init { _name = value; }
        }

        public Guid Guid
        {
            get { return _guid; }
            init { _guid = value; }
        }


        public LinkModelInWPF(string name, Guid guid, bool isMonitoring)
        {
            Name = name;
            Guid = guid;
            IsMonitoring = isMonitoring;
            ClickCommand = new RelayCommand(() => ToggleMonitoring(this, null)); // Привязка команды клика к методу
        }
        public static LinkModelInWPF[] GetLinks(Guid revitDocumentGuid)
        {
            // TODO: Implement logic to retrieve links from the application context or configuration.
            return new LinkModelInWPF[]
            {
                
            };
        }
        private void ToggleMonitoring(object sender, MouseButtonEventArgs? e)
        { 
            ((LinkModelInWPF)sender).IsMonitoring = !((LinkModelInWPF)sender).IsMonitoring; // Переключение состояния отслеживания при клике
        }
    }
}
