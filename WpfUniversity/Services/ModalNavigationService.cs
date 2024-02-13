using System;
using WpfUniversity.ViewModels;

namespace WpfUniversity.Services;

public class ModalNavigationService
{
    private ViewModelBase _currentViewModel;

    public ViewModelBase CurrentViewModel
    {
        get
        {
            return _currentViewModel;
        }
        set
        {
            _currentViewModel = value;
            CurrentViewModelChanged?.Invoke();
        }
    }

    public bool IsOpen => CurrentViewModel != null;


    public event Action CurrentViewModelChanged;

    public void Close()
    {
        CurrentViewModel = null;
    }
}
