using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using UniversityDataLayer.Entities;
using WpfUniversity.Commands;
using WpfUniversity.Services.Interfaces;

namespace WpfUniversity.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ICourseService _courseService;
    private readonly IWindowService _windowService;

    public MainViewModel(ICourseService courseService, IWindowService windowService)
    {
        _courseService = courseService;
        _windowService = windowService;

        Courses = new ObservableCollection<Course>();
        LoadCoursesCommand = new AsyncRelayCommand(LoadCourses);
        NextPageCoursesCommand = new RelayCommand(NextPageCourses, () => CanGoToNextPageCourses);
        PreviousPageCoursesCommand = new RelayCommand(PreviousPageCourses, () => CanGoToPreviousPageCourses);
    }

    public ObservableCollection<Course> Courses { get; set; }

    private Course _selectedCourse;
    public Course SelectedCourse
    {
        get => _selectedCourse;
        set
        {
            if (SetProperty(ref _selectedCourse, value) && value != null)
            {
                _windowService.OpenGroupsWindow(_selectedCourse);
            }
        }
    }

    private int _currentPageCourses = 1;
    private int _itemsPerPageCourses = 10;
    private int _totalCourses;

    public int CurrentPageCourses
    {
        get => _currentPageCourses;
        set
        {
            if (SetProperty(ref _currentPageCourses, value))
            {
                UpdateCoursesCollection();
            }
        }
    }

    public bool CanGoToNextPageCourses => _currentPageCourses * _itemsPerPageCourses < _totalCourses;
    public bool CanGoToPreviousPageCourses => _currentPageCourses > 1;

    public ICommand LoadCoursesCommand { get; }
    public ICommand NextPageCoursesCommand { get; }
    public ICommand PreviousPageCoursesCommand { get; }

    public async Task LoadCourses()
    {
        await _courseService.Load();

        _totalCourses = _courseService.Courses.Count;
        UpdateCoursesCollection();
    }

    private void UpdateCoursesCollection()
    {
        var pagedCourses = _courseService.Courses
            .Skip((_currentPageCourses - 1) * _itemsPerPageCourses)
            .Take(_itemsPerPageCourses);

        Courses.Clear();
        foreach (var course in pagedCourses)
        {
            Courses.Add(course);
        }

        OnPropertyChanged(nameof(CanGoToNextPageCourses));
        OnPropertyChanged(nameof(CanGoToPreviousPageCourses));
    }

    private void NextPageCourses()
    {
        _currentPageCourses++;
        UpdateCoursesCollection();
    }

    private void PreviousPageCourses()
    {
        if (_currentPageCourses > 1)
        {
            _currentPageCourses--;
            UpdateCoursesCollection();
        }
    }
}
