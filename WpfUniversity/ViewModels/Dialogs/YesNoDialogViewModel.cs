using System.Windows;
using System.Windows.Input;
using WpfUniversity.Command;
using WpfUniversity.Services;

namespace WpfUniversity.ViewModels.Dialogs;

public class YesNoDialogViewModel : ViewModelBase
{
    private string _message;
    private readonly ModalNavigationService _modalNavigationService;

    public string Message
    {
        get { return _message; }
        set { _message = value; OnPropertyChanged(nameof(Message)); }
    }

    public ICommand ShowDialogCommand { get; }

    public bool UserChoice { get; private set; }

    public YesNoDialogViewModel(ModalNavigationService modalNavigationService)
    {
        ShowDialogCommand = new RelayCommand(ShowDialog);
        _modalNavigationService = modalNavigationService;
    }

    private void ShowDialog(object parameter)
    {
        var result = MessageBox.Show(Message, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            UserChoice = true;
        }
        else
        {
            UserChoice = false;
        }

        _modalNavigationService.Close();
    }
}
