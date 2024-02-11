using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUniversity.Command;
using WpfUniversity.Services;

namespace WpfUniversity.ViewModels.Dialogs
{
    public class YesNoDialogViewModel : ViewModelBase
    {
        private string _message;
        private readonly ModalNavigationService _modalNavigationService;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public ICommand YesCommand { get; }
        public ICommand NoCommand { get; }

        public bool UserChoice { get; private set; }

        public YesNoDialogViewModel(ModalNavigationService modalNavigationService)
        {
            YesCommand = new RelayCommand(Yes);
            NoCommand = new RelayCommand(No);
            _modalNavigationService = modalNavigationService;
        }

        private void Yes(object parameter)
        {
            UserChoice = true;
            _modalNavigationService.Close();
        }

        private void No(object parameter)
        {
            UserChoice = false;
            _modalNavigationService.Close();
        }
    }
}
