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

        public string Name { get; init; }
        public Guid Guid { get; init; }

        Status _status;
        public Status Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        LinkModelInWPF[]? _linksInDocument;
        public LinkModelInWPF[]? LinksInDocument
        {
            get => _linksInDocument;
            set
            {
                if (_linksInDocument is null) _linksInDocument = value;
                   
            }
        }

        public DocumentModelInWPF(string name, Guid guid, Status status)
        {
            Name = name;
            Guid = guid;
            Status = status;

        }

    }
}
