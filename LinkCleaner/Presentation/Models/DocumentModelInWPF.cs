using System.ComponentModel;
using System.Runtime.CompilerServices;

using LinkCleaner.Presentation.Interfaces;


namespace LinkCleaner.Presentation.Models
{
    public class DocumentModelInWPF : INotifyPropertyChanged, IDocumentViewData
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string? _name;
        public string Name 
        {
            get
            {
                if (_name is null)
                {
                    return "Unnamed Document";
                }
                return _name;
            }
            set => _name = value;
        }
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

        List<ILinkViewData>? _linksInDocument;
        public List<ILinkViewData>? LinksInDocument
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
