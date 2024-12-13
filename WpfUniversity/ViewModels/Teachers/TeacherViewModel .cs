using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using UniversityDataLayer.Entities;
using WpfUniversity.Commands;
using WpfUniversity.Services.Interfaces;

namespace WpfUniversity.ViewModels.Teachers;

public class TeacherViewModel : ViewModelBase
{
    private readonly IWindowService _windowService;
    private readonly ITeacherService _teacherService;
    private readonly ICourseService _courseService;

    private Teacher _teacher;
    public Teacher Teacher
    {
        get => _teacher;
        set => SetProperty(ref _teacher, value);
    }

    private ObservableCollection<Course> _courses;
    public ObservableCollection<Course> Courses
    {
        get => _courses;
        set => SetProperty(ref _courses, value);
    }

    private string _windowTitle;
    public string WindowTitle
    {
        get => _windowTitle;
        set => SetProperty(ref _windowTitle, value);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                ((AsyncRelayCommand)SaveCommand).RaiseCanExecuteChanged();
                ((RelayCommand)CancelCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action<bool> CloseRequested;

    public TeacherViewModel(IWindowService windowService, ITeacherService teacherService, ICourseService courseService, Teacher teacher = null)
    {
        _windowService = windowService;
        _teacherService = teacherService;
        _courseService = courseService;

        Teacher = teacher ?? new Teacher();
        WindowTitle = teacher == null ? "Add Teacher" : "Edit Teacher";

        SaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new RelayCommand(Cancel, () => !IsBusy);

        Task.Run(async () => await LoadCoursesAsync());
    }

    private async Task LoadCoursesAsync()
    {
        try
        {
            IsBusy = true;
            await _courseService.Load();
            Courses = new ObservableCollection<Course>(_courseService.Courses);
        }
        catch (Exception ex)
        {
            _windowService.ShowMessageDialog($"Error loading courses: {ex.Message}", "Error");

        }
        finally
        {
            IsBusy = false;
        }
    }

    public bool CanSave
    {
        get
        {
            return !string.IsNullOrWhiteSpace(Teacher.FirstName) && !string.IsNullOrWhiteSpace(Teacher.LastName) && Teacher.Course != null;
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            IsBusy = true;

            if (string.IsNullOrWhiteSpace(Teacher.FirstName) || string.IsNullOrWhiteSpace(Teacher.LastName) || Teacher.Course == null)
            {
                _windowService.ShowMessageDialog("All fields are reqieried", "Validation Error");
                return;
            }

            if (Teacher.Id == 0)
            {
                var teacherToAdd = new Teacher()
                {
                    FirstName = Teacher.FirstName,
                    LastName = Teacher.LastName,
                    CourseId = Teacher.Course.Id,
                    Subject = Teacher.Subject,
                };
                await _teacherService.AddTeacherAsync(teacherToAdd);
                _windowService.ShowMessageDialog("Teacher added successfully.", "Success");
            }
            else
            {
                await _teacherService.UpdateTeacherAsync(Teacher);
                _windowService.ShowMessageDialog("Teacher updated successfully.", "Success");
            }

            CloseRequested?.Invoke(true);
        }
        catch (Exception ex)
        {
            _windowService.ShowMessageDialog($"Error saving teacher: {ex.Message}", "Error");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Cancel()
    {
        CloseRequested?.Invoke(false);
    }
}
