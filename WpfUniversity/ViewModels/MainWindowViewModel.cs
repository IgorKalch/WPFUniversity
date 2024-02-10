using MvvmCross.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using UniversityDataLayer.Entities;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.Services;
using WpfUniversity.ViewModels.Courses;

namespace WpfUniversity.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ModalNavigationService _modalNavigationService;

    public ViewModelBase CurrentModalViewModel => _modalNavigationService.CurrentViewModel;
    public bool IsModalOpen => _modalNavigationService.IsOpen;

    public CourseViewModel CourseViewModel { get; }

    public MainWindowViewModel(ModalNavigationService modalNavigationService, CourseViewModel courseViewModel)
    {
        _modalNavigationService = modalNavigationService;
        CourseViewModel = courseViewModel;

        _modalNavigationService.CurrentViewModelChanged += ModalNavigationService_CurrentViewModelChanged;
    }

    protected override void Dispose()
    {
        _modalNavigationService.CurrentViewModelChanged -= ModalNavigationService_CurrentViewModelChanged;

        base.Dispose();
    }

    private void ModalNavigationService_CurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentModalViewModel));
        OnPropertyChanged(nameof(IsModalOpen));
    }
}
