using System.Windows.Input;
using UniversityDataLayer.Entities;
using WpfUniversity.Command.Groups;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;
using WpfUniversity.Services.Groups;
using WpfUniversity.StartUpHelpers;
using WpfUniversity.ViewModels.Groups;

namespace WpfUniversity.ViewModels.Courses;

public class CourseDetailsViewModel : ViewModelBase
{
    private SelectedCourseService _selectedCourse;
    private SelectedGroupService _selectedGroup;
    private string? _errorMessage;
    private bool _hasErrorMessage;

    public Course SelectedCourse => _selectedCourse.SelectedCourse;
    public Group SelectedGroup => _selectedGroup.SelectedGroup;
    public bool IsSelectedGroup => SelectedGroup != null;
    public int TeacherCount => SelectedCourse?.Teachers?.Count ?? 0;
    public int GroupCount => SelectedCourse?.Groups?.Count ?? 0;

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

    public ICommand OpenGroupCommand { get; set; }

    public CourseDetailsViewModel(SelectedCourseService selectedCourse, SelectedGroupService selectedGroup, ModalNavigationService modalNavigationService)
    {
        _selectedCourse = selectedCourse;
        _selectedCourse.SelectedCourseChanged += SelectedCourseService_SelectedCourseChanged;
        _selectedGroup = selectedGroup;
        _selectedGroup.SelectedGroupChanged += SelectedGroupService_SelectedGroupChanged;

        OpenGroupCommand = new OpenGroupCommand(selectedGroup.SelectedGroup, modalNavigationService);
    }

    protected override void Dispose()
    {
        _selectedCourse.SelectedCourseChanged -= SelectedCourseService_SelectedCourseChanged;
        _selectedGroup.SelectedGroupChanged -= SelectedGroupService_SelectedGroupChanged;

        base.Dispose();
    }

    private void SelectedCourseService_SelectedCourseChanged()
    {
        OnPropertyChanged(nameof(SelectedCourse));
        OnPropertyChanged(nameof(TeacherCount));
        OnPropertyChanged(nameof(GroupCount));
    }

    private void SelectedGroupService_SelectedGroupChanged()
    {
        OnPropertyChanged(nameof(SelectedGroup));
        OnPropertyChanged(nameof(IsSelectedGroup));
    }
}
