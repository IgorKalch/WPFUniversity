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

namespace WpfUniversity.ViewModels.Teachers;

public class TeachersViewModel : ViewModelBase
{
    private readonly IWindowService _windowService;
    private readonly ITeacherService _teacherService;
    private readonly ICourseService _courseService;

    private ObservableCollection<Teacher> _teachers;
    public ObservableCollection<Teacher> Teachers
    {
        get => _teachers;
        set => SetProperty(ref _teachers, value);
    }

    private Teacher _selectedTeacher;
    public Teacher SelectedTeacher
    {
        get => _selectedTeacher;
        set
        {
            if (SetProperty(ref _selectedTeacher, value))
            {
                OnPropertyChanged(nameof(IsTeacherSelected));
                ((AsyncRelayCommand)EditTeacherCommand).RaiseCanExecuteChanged();
                ((AsyncRelayCommand)DeleteTeacherCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsTeacherSelected => SelectedTeacher != null;

    public bool CanDeleteTeacher => IsTeacherSelected;

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                ((AsyncRelayCommand)AddTeacherCommand).RaiseCanExecuteChanged();
                ((AsyncRelayCommand)EditTeacherCommand).RaiseCanExecuteChanged();
                ((AsyncRelayCommand)DeleteTeacherCommand).RaiseCanExecuteChanged();
            }
        }
    }

    private ObservableCollection<Course> _courses;
    public ObservableCollection<Course> Courses
    {
        get => _courses;
        set => SetProperty(ref _courses, value);
    }

    public ICommand AddTeacherCommand { get; }
    public ICommand EditTeacherCommand { get; }
    public ICommand DeleteTeacherCommand { get; }
    public ICommand NextPageTeachersCommand { get; }
    public ICommand PreviousPageTeachersCommand { get; }
    public ICommand LoadTeacherCommand { get; }

    public ICommand SortCommand { get; }

    public TeachersViewModel(IWindowService windowService, ITeacherService teacherService, ICourseService courseService)
    {
        _windowService = windowService;
        _teacherService = teacherService;
        _courseService = courseService;

        Teachers = new ObservableCollection<Teacher>();
        Courses = new ObservableCollection<Course>();

        AddTeacherCommand = new AsyncRelayCommand(AddTeacherAsync, () => !IsBusy);
        EditTeacherCommand = new AsyncRelayCommand(EditTeacherAsync, () => IsTeacherSelected && !IsBusy);
        DeleteTeacherCommand = new AsyncRelayCommand(DeleteTeacherAsync, () => (CanDeleteTeacher && !IsBusy));
        LoadTeacherCommand = new AsyncRelayCommand(LoadTeachersAsync);

        SortCommand = new RelayCommand<DataGridSortingEventArgs>(OnSortCommandExecuted);
        NextPageTeachersCommand = new RelayCommand(NextPageTeachers, () => CanGoToNextPageTeachers);
        PreviousPageTeachersCommand = new RelayCommand(PreviousPageTeachers, () => CanGoToPreviousPageTeachers);

        LoadTeacherCommand.Execute(this);
    }


    private int _currentPageTeachers = 1;
    private int _itemsPerPageTeachers = 20;
    private int _totalTeachers;
    public int PageSizeTeachers
    {
        get => _itemsPerPageTeachers;
        set
        {
            if (SetProperty(ref _itemsPerPageTeachers, value))
            {
                UpdateTeachersCollection();
            }
        }
    }
    public int CurrentPageTeachers
    {
        get => _currentPageTeachers;
        set
        {
            if (SetProperty(ref _currentPageTeachers, value))
            {
                UpdateTeachersCollection();
            }
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
                CurrentPageTeachers = 1;
                UpdateTeachersCollection();
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
                CurrentPageTeachers = 1;
                UpdateTeachersCollection();
            }
        }
    }


    public bool CanGoToNextPageTeachers => _currentPageTeachers * _itemsPerPageTeachers < _totalTeachers;
    public bool CanGoToPreviousPageTeachers => _currentPageTeachers > 1;


    private async Task LoadTeachersAsync()
    {
        try
        {
            IsBusy = true;
            await _teacherService.Load();
            await _courseService.Load();

            _totalTeachers = _teacherService.Teachers.Count;


            UpdateTeachersCollection();
        }
        catch (Exception ex)
        {
            _windowService.ShowMessageDialog($"Error loading teachers: {ex.Message}", "Error");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddTeacherAsync()
    {
        var teacher = new Teacher();
        var isSaved = _windowService.OpenTeacherDialog(null, "Add Teacher");
        if (isSaved)
        {
            try
            {
                IsBusy = true;

                LoadTeacherCommand.Execute(this);
            }
            catch (Exception ex)
            {
                _windowService.ShowMessageDialog($"Error adding teacher: {ex.Message}", "Error");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    private async Task EditTeacherAsync()
    {
        if (SelectedTeacher == null)
            return;

        var isSaved = _windowService.OpenTeacherDialog(SelectedTeacher, "Edit Teacher");
        if (isSaved)
        {
            try
            {
                IsBusy = true;
                LoadTeacherCommand.Execute(this);
                OnPropertyChanged(nameof(SelectedTeacher));
            }
            catch (Exception ex)
            {
                _windowService.ShowMessageDialog($"Error updating teacher: {ex.Message}", "Error");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    private async Task DeleteTeacherAsync()
    {
        if (SelectedTeacher == null)
            return;

        bool confirm = _windowService.ShowConfirmationDialog($"Are you sure you want to delete {SelectedTeacher.FullName}?", "Confirm Delete");
        if (confirm)
        {
            try
            {
                IsBusy = true;
                await _teacherService.DeleteTeacherAsync(SelectedTeacher);
                Teachers.Remove(SelectedTeacher);
                _windowService.ShowMessageDialog("Teacher deleted successfully.", "Success");
            }
            catch (Exception ex)
            {
                _windowService.ShowMessageDialog($"Error deleting teacher: {ex.Message}", "Error");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    private void UpdateTeachersCollection()
    {
        IEnumerable<Teacher> sortedTeachers = _teacherService.Teachers;

        if (!string.IsNullOrEmpty(SortColumn))
        {
            switch (SortColumn)
            {
                case "Id":
                    sortedTeachers = SortAscending ? sortedTeachers.OrderBy(c => c.Id) : sortedTeachers.OrderByDescending(c => c.Id);
                    break;
                case "FullName":
                    sortedTeachers = SortAscending ? sortedTeachers.OrderBy(c => c.FullName) : sortedTeachers.OrderByDescending(c => c.FullName);
                    break;
                case "Subject":
                    sortedTeachers = SortAscending ? sortedTeachers.OrderBy(c => c.Subject) : sortedTeachers.OrderByDescending(c => c.Subject);
                    break;
                case "CourseName":
                    sortedTeachers = SortAscending ? sortedTeachers.OrderBy(c => c.Course.Name) : sortedTeachers.OrderByDescending(c => c.Course.Name);
                    break;
                default:
                    break;
            }
        }

        var pagedCourses = sortedTeachers
            .Skip((_currentPageTeachers - 1) * _itemsPerPageTeachers)
            .Take(_itemsPerPageTeachers)
            .ToList();

        Teachers.Clear();
        foreach (var teacher in pagedCourses)
        {
            Teachers.Add(teacher);
        }

        OnPropertyChanged(nameof(CanGoToNextPageTeachers));
        OnPropertyChanged(nameof(CanGoToPreviousPageTeachers));
    }

    private void NextPageTeachers()
    {
        _currentPageTeachers++;
        UpdateTeachersCollection();
    }

    private void PreviousPageTeachers()
    {
        if (_currentPageTeachers > 1)
        {
            _currentPageTeachers--;
            UpdateTeachersCollection();
        }
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

        UpdateTeachersCollection();
    }
}
