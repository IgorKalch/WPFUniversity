using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        Courses = new ObservableCollection<Course>();

        SaveCommand = new AsyncRelayCommand(SaveAsync, () => CanSave);
        CancelCommand = new RelayCommand(Cancel, () => !IsBusy);

        Task.Run(async () => await LoadCoursesAsync());
    }

    private async Task LoadCoursesAsync()
    {
        try
        {
            IsBusy = true;
            var courses = await _courseService.GetAllCoursesAsync();
            Courses.Clear();
            foreach (var course in courses)
            {
                Courses.Add(course);
            }

            if (Teacher.Course != null)
            {
                var existingCourse = Courses.FirstOrDefault(c => c.Id == Teacher.CourseId);
                if (existingCourse != null)
                {
                    Teacher.Course = existingCourse;
                }
            }
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
            return !IsBusy && !string.IsNullOrWhiteSpace(Teacher.FirstName) && !string.IsNullOrWhiteSpace(Teacher.LastName) && Teacher.Course != null;
        }
    }

    private async Task SaveAsync()
    {
        if (!CanSave)
            return;

        try
        {
            IsBusy = true;

            if (Teacher.Id == 0)
            {
                await _teacherService.AddTeacherAsync(Teacher);
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
