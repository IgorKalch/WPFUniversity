using MvvmCross.Commands;
using System;
using System.Threading.Tasks;
using System.Windows.Navigation;
using UniversityDataLayer.Entities;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;

namespace WpfUniversity.ViewModels.Courses;

public class AddCourseViewModel : ViewModelBase
{
    private readonly CourseService _courseService;
    private readonly ModalNavigationService _modalNavigationService;
    private string _name;
    private string _description;
    private string? _errorMessage;
    private bool _hasErrorMessage;
    private bool _canSubmit;

    public string Name
    {
        get { return _name; }
        set
        {
            SetProperty(ref _name, value);
            SetProperty<bool>(ref _canSubmit, !string.IsNullOrEmpty(value), nameof(CanAddCourse));
        }
    }
    public string Description
    {
        get { return _description; }
        set
        {
            SetProperty(ref _description, value);
        }
    }

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

    public bool CanAddCourse => Name?.Length > 3;

    public IMvxCommand AddCourseCommand { get; set; }

    public AddCourseViewModel(CourseService courseService, ModalNavigationService modalNavigationService)
    {
        _courseService = courseService;
        _modalNavigationService = modalNavigationService;

        AddCourseCommand = new MvxCommand(AddCourse);
    }

    private async void AddCourse()
    {
        _modalNavigationService.CurrentViewModel = this;

       ErrorMessage = null;

        Course course = new Course();
        course.Name = Name;
        course.Description = Description;

        try
        {
            await _courseService.Add(course);

            _modalNavigationService.Close();
        }
        catch (Exception)
        {
            ErrorMessage = "Failed to add Course. Please try again later.";
        }
    }
}
