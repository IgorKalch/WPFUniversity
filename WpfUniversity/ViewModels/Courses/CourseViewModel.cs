using System.Windows.Input;
using WpfUniversity.Command.Courses;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;

namespace WpfUniversity.ViewModels.Courses;

public  class CourseViewModel : ViewModelBase
{  
    private readonly CourseService _courseService;
    private readonly ModalNavigationService _modalNavigationService;
    private bool _isLoading;
    private string? _errorMessage;
    private bool _hasErrorMessage;

    public CourseDetailsViewModel CourseDetailsViewModel { get;  }
    public CourseTreeViewModel CourseTreeViewModel { get;  }

    public ICommand AddCourseCommand { get; }
    public ICommand LoadCourseCommand { get; }
    
    public string? ErrorMessage
    {
        get { return _errorMessage; }
        set
        {
            _errorMessage = value;
            OnPropertyChanged(nameof(HasErrorMessage));
        }
    }

    public bool HasErrorMessage
    {
        get { return _hasErrorMessage; }
        set { _hasErrorMessage = value; }
    }
    public bool IsLoading
    {
        get { return _isLoading; }
        set { _isLoading = value; }
    }


    public CourseViewModel(CourseService courseService, SelectedCourseService selectedCourseService, ModalNavigationService modalNavigationService)
    {
        _courseService = courseService;
        _modalNavigationService = modalNavigationService;

        CourseTreeViewModel = new CourseTreeViewModel(courseService, selectedCourseService, modalNavigationService);
        CourseDetailsViewModel = new CourseDetailsViewModel(selectedCourseService);

        
        LoadCourseCommand = new LoadCourseCommand(this, courseService);
        AddCourseCommand = new OpenAddCourseCommand(courseService, modalNavigationService);
    }

    public static CourseViewModel LoadViewModel(CourseService courseService, SelectedCourseService selectedCourseService, ModalNavigationService modalNavigationService)
    {
        CourseViewModel viewModel = new CourseViewModel(courseService, selectedCourseService, modalNavigationService);

        viewModel.LoadCourseCommand.Execute(null);

        return viewModel;
    }
}
