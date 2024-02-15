using System.Windows.Input;
using WpfUniversity.Command.Courses;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;
using WpfUniversity.Services.Groups;

namespace WpfUniversity.ViewModels.Courses;

public  class CourseViewModel : ViewModelBase
{  
    private bool _isLoading;
    private string? _errorMessage;
    private bool _hasErrorMessage;

    public CourseDetailsViewModel CourseDetailsViewModel { get;  }
    public CourseTreeViewModel CourseTreeViewModel { get;  }

    public ICommand AddCourseCommand { get; }
    public ICommand EditCourseCommand { get; }
    public ICommand DeleteCourseCommand { get; }
    public ICommand LoadCourseCommand { get; }
    
    public string? ErrorMessage
    {
        get { return _errorMessage; }
        set
        {
            _errorMessage = value;
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(HasErrorMessage));
        }
    }
    public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

    public bool IsLoading
    {
        get { return _isLoading; }
        set { _isLoading = value; }
    }    

    public CourseViewModel(CourseService courseService, SelectedCourseService selectedCourseService, GroupService groupService, SelectedGroupService selectedGroupService
        , ModalNavigationService modalNavigationService)
    {
        CourseTreeViewModel = new CourseTreeViewModel(courseService, selectedCourseService, groupService, selectedGroupService,  modalNavigationService);
        CourseDetailsViewModel = new CourseDetailsViewModel(selectedCourseService, selectedGroupService, modalNavigationService);

        
        LoadCourseCommand = new LoadCourseCommand(this, courseService);
        AddCourseCommand = new OpenAddCourseCommand(courseService, modalNavigationService);
        EditCourseCommand = new OpenEditCourseCommand(CourseTreeViewModel, courseService, modalNavigationService);
        DeleteCourseCommand = new DeleteCourseCommand(this, courseService, modalNavigationService);
    }

    public static CourseViewModel LoadViewModel(CourseService courseService, SelectedCourseService selectedCourseService, GroupService groupService, SelectedGroupService selectedGroupService, ModalNavigationService modalNavigationService)
    {
        CourseViewModel viewModel = new CourseViewModel(courseService, selectedCourseService, groupService, selectedGroupService, modalNavigationService);

        viewModel.LoadCourseCommand.Execute(null);

        return viewModel;
    }
}
