
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using LinkCleaner.Presentation.Emuns;

namespace LinkCleaner.Presentation.Models
{
    public class DocumentModelInWPF : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public string? Name { get; set; }
        public Guid Guid { get; init; }

        bool _status;
        public bool Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        List<LinkModelInWPF>? _linksInDocument;
        public List<LinkModelInWPF>? LinksInDocument
        {
            get => _linksInDocument;
            init
            {
                _linksInDocument = value;
            }
        }

        public DocumentModelInWPF(string? name, Guid guid, bool status)
        {
            Name = name;
            Guid = guid;
            Status = status;

        }

    }
}
