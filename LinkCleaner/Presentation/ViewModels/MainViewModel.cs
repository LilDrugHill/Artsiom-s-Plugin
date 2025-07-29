using System.ComponentModel;
using System.Windows.Input;

using LinkCleaner.Presentation.Commands;
using LinkCleaner.Presentation.Interfaces;

namespace LinkCleaner.Presentation.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        IDocumentViewData _document;
        public event PropertyChangedEventHandler? PropertyChanged;
        public IDocumentViewData Document
        {
            get { return _document; }
            set { _document = value; }
        }
        public MainViewModel()
        {
            CloseCommand = new RelayCommand(() => RequestClose?.Invoke(this, EventArgs.Empty));
            ApplyCommand = new RelayCommand(() => OnApplyChangesToConfig?.Invoke(this, EventArgs.Empty));
            ToggleStatus = new RelayCommand(() => OnToggleDocumentStatus?.Invoke(this, EventArgs.Empty));
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICommand CloseCommand { get; }
        public ICommand ApplyCommand { get; }
        public ICommand ToggleStatus { get; }
        public event EventHandler RequestClose;
        public event EventHandler OnApplyChangesToConfig;
        public event EventHandler OnToggleDocumentStatus;



    }
}
