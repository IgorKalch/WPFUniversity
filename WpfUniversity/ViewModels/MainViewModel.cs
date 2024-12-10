using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using UniversityDataLayer.Entities;
using WpfUniversity.Commands;
using WpfUniversity.Services.Interfaces;

namespace WpfUniversity.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ICourseService _courseService;
    private readonly IGroupService _groupService;
    private readonly IWindowService _windowService;

    public MainViewModel(ICourseService courseService, IGroupService groupService, IWindowService windowService)
    {
        _courseService = courseService;
        _groupService = groupService;
        _windowService = windowService;

        Courses = new ObservableCollection<Course>();

        LoadCoursesCommand = new AsyncRelayCommand(LoadCourses);
        NextPageCoursesCommand = new RelayCommand(NextPageCourses, () => CanGoToNextPageCourses);
        PreviousPageCoursesCommand = new RelayCommand(PreviousPageCourses, () => CanGoToPreviousPageCourses);

        AddCourseCommand = new RelayCommand(AddCourse);
        EditCourseCommand = new RelayCommand(EditCourse, () => IsCourseSelected);
        DeleteCourseCommand = new RelayCommand(DeleteCourse, () => CanDeleteCourse);

        OpenGroupsCommand = new RelayCommand(OpenGroups, () => SelectedCourse != null);
        SortCommand = new RelayCommand<DataGridSortingEventArgs>(OnSortCommandExecuted);

        OpenTeacherWindowCommand = new RelayCommand(OpenTeachersWindowAsync);

    }

    public ObservableCollection<Course> Courses { get; set; }

    private Course _selectedCourse;
    public Course SelectedCourse
    {
        get => _selectedCourse;
        set
        {
            if (SetProperty(ref _selectedCourse, value))
            {
                OnPropertyChanged(nameof(IsCourseSelected));
                OnPropertyChanged(nameof(CanDeleteCourse));

                ((RelayCommand)EditCourseCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCourseCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsCourseSelected => SelectedCourse != null;

    public bool CanDeleteCourse
    {
        get
        {
            if (SelectedCourse == null)
                return false;

            var isHasGroup = SelectedCourse.Groups?.Any() ?? false;

            return !isHasGroup;
        }
    }

    private string _sortColumn;
    private bool _sortAscending = true;

    public string SortColumn
    {
        get => _sortColumn;
        set
        {
            if (SetProperty(ref _sortColumn, value))
            {
                CurrentPageCourses = 1;
                UpdateCoursesCollection();
            }
        }
    }

    public bool SortAscending
    {
        get => _sortAscending;
        set
        {
            if (SetProperty(ref _sortAscending, value))
            {
                CurrentPageCourses = 1;
                UpdateCoursesCollection();
            }
        }
    }

    private int _currentPageCourses = 1;
    private int _itemsPerPageCourses = 20;
    private int _totalCourses;
    public int PageSizeCourses
    {
        get => _itemsPerPageCourses;
        set
        {
            if (SetProperty(ref _itemsPerPageCourses, value))
            {
                UpdateCoursesCollection();
            }
        }
    }
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

    public ICommand SortCommand { get; }
    public ICommand OpenGroupsCommand { get; }
    public ICommand LoadCoursesCommand { get; }
    public ICommand NextPageCoursesCommand { get; }
    public ICommand PreviousPageCoursesCommand { get; }
    public ICommand AddCourseCommand { get; }
    public ICommand EditCourseCommand { get; }
    public ICommand DeleteCourseCommand { get; }
    public ICommand OpenTeacherWindowCommand { get; }

    public async Task LoadCourses()
    {
        await _courseService.Load();

        _totalCourses = _courseService.Courses.Count;
        UpdateCoursesCollection();
    }

    private void OnSortCommandExecuted(DataGridSortingEventArgs e)
    {
        var column = e.Column;

        e.Handled = true;

        var sortMemberPath = column.SortMemberPath;
        if (string.IsNullOrEmpty(sortMemberPath))
        {
            return;
        }

        if (SortColumn == sortMemberPath)
        {
            SortAscending = !SortAscending;
        }
        else
        {
            SortColumn = sortMemberPath;
            SortAscending = true;
        }

        column.SortDirection = SortAscending ? ListSortDirection.Ascending : ListSortDirection.Descending;

        UpdateCoursesCollection();
    }

    private void OpenGroups()
    {
        if (SelectedCourse != null)
        {
            _windowService.OpenGroupsWindow(SelectedCourse);
        }
    }

    private void UpdateCoursesCollection()
    {
        IEnumerable<Course> sortedCourses = _courseService.Courses;

        if (!string.IsNullOrEmpty(SortColumn))
        {
            switch (SortColumn)
            {
                case "Id":
                    sortedCourses = SortAscending ? sortedCourses.OrderBy(c => c.Id) : sortedCourses.OrderByDescending(c => c.Id);
                    break;
                case "Name":
                    sortedCourses = SortAscending ? sortedCourses.OrderBy(c => c.Name) : sortedCourses.OrderByDescending(c => c.Name);
                    break;
                case "Description":
                    sortedCourses = SortAscending ? sortedCourses.OrderBy(c => c.Description) : sortedCourses.OrderByDescending(c => c.Description);
                    break;
                default:
                    break;
            }
        }

        var pagedCourses = sortedCourses
            .Skip((_currentPageCourses - 1) * _itemsPerPageCourses)
            .Take(_itemsPerPageCourses)
            .ToList();

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

    private void AddCourse()
    {
        _windowService.OpenAddCourseWindow();
        LoadCoursesCommand.Execute(null);
    }

    private void EditCourse()
    {
        if (SelectedCourse == null)
            return;

        _windowService.OpenEditCourseWindow(SelectedCourse);
        LoadCoursesCommand.Execute(null);
    }

    private void DeleteCourse()
    {
        if (SelectedCourse == null)
            return;

        var result = _windowService.ShowConfirmationDialog("Are you sure you want to delete this course?", "Delete Confirmation");
        if (result)
        {
            _courseService.Delete(SelectedCourse);
            LoadCoursesCommand.Execute(null);
        }
    }
    private void OpenTeachersWindowAsync()
    {
        try
        {
            _windowService.OpenTeachersWindow();
        }
        catch (Exception ex)
        {
            _windowService.ShowMessageDialog($"Error opening Teacher Window: {ex.Message}", "Error");
        }
    }
}
