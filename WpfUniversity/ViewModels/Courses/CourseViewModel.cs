using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;

namespace WpfUniversity.ViewModels.Courses;

public  class CourseViewModel : MvxViewModel
{  
    private readonly IUnitOfWork _unit;
    private readonly CourseService _courseService;
    private readonly ModalNavigationService _modalNavigationService;
    private bool _isLoading;
    private string? _errorMessage;
    private bool _hasErrorMessage;

    public CourseDetailsViewModel CourseDetailsViewModel { get;  }
    public CourseTreeViewModel CourseTreeViewModel { get;  }
    public AddCourseViewModel AddCourseViewModel { get; }

    public IMvxCommand AddCourseCommand { get; }
    public IMvxCommand LoadCourseCommand { get; }
    
    public string? ErrorMessage
    {
        get { return _errorMessage; }
        set
        {
            SetProperty(ref _errorMessage, value);
            SetProperty<bool>(ref _hasErrorMessage, !string.IsNullOrEmpty(value), nameof(HasErrorMessage));
        }
    }

    public bool HasErrorMessage
    {
        get { return _hasErrorMessage; }
        set { SetProperty(ref _hasErrorMessage, value); }
    }
    public bool IsLoading
    {
        get { return _isLoading; }
        set { SetProperty(ref _isLoading, value); }
    }


    public CourseViewModel( IUnitOfWork unitOfWork, CourseService courseService, SelectedCourseService selectedCourseService, ModalNavigationService modalNavigationService)
    {
        _unit = unitOfWork;
        _courseService = courseService;
        _modalNavigationService = modalNavigationService;

        CourseTreeViewModel = new CourseTreeViewModel(_unit, courseService, selectedCourseService, modalNavigationService);
        CourseDetailsViewModel = new CourseDetailsViewModel(selectedCourseService);
        AddCourseViewModel = new AddCourseViewModel(_courseService, _modalNavigationService);

        AddCourseCommand = AddCourseViewModel.AddCourseCommand;
        LoadCourseCommand = new MvxCommand(Load);
    }

    public static CourseViewModel LoadViewModel(IUnitOfWork unitOfWork, CourseService courseService, SelectedCourseService selectedCourseService, ModalNavigationService modalNavigationService)
    {
        CourseViewModel viewModel = new CourseViewModel(unitOfWork, courseService, selectedCourseService, modalNavigationService);

        viewModel.LoadCourseCommand.Execute();

        return viewModel;
    }

    private async void Load()
    {

        ErrorMessage = null;
        IsLoading = true;

        try
        {
            await _courseService.Load();
        }
        catch (Exception)
        {
            ErrorMessage = "Failed to load Course. Please restart the application.";
        }
    }
}
