using System;
using System.Windows.Input;
using WpfUniversity.Commands;

namespace WpfUniversity.ViewModels.Dialogs;

public class ErrorDialogViewModel : ViewModelBase
{
    public ErrorDialogViewModel()
    {
        OkCommand = new RelayCommand(Ok);
    }

    private string _title;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string _message;
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public ICommand OkCommand { get; }

    public Action OnOk { get; set; }

    private void Ok()
    {
        OnOk?.Invoke();
    }
}
