using System;
using System.Windows.Input;
using WpfUniversity.Commands;

namespace WpfUniversity.ViewModels.Dialogs;

public class ConfirmationDialogViewModel : ViewModelBase
{
    public ConfirmationDialogViewModel()
    {
        ConfirmCommand = new RelayCommand(Confirm);
        CancelCommand = new RelayCommand(Cancel);
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

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public Action OnConfirm { get; set; }
    public Action OnCancel { get; set; }

    private void Confirm()
    {
        OnConfirm?.Invoke();
    }

    private void Cancel()
    {
        OnCancel?.Invoke();
    }
}