using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

    public TeachersViewModel(IWindowService windowService, ITeacherService teacherService, ICourseService courseService)
    {
        _windowService = windowService;
        _teacherService = teacherService;
        _courseService = courseService;

        Teachers = new ObservableCollection<Teacher>();
        Courses = new ObservableCollection<Course>();

        AddTeacherCommand = new AsyncRelayCommand(AddTeacherAsync, () => !IsBusy);
        EditTeacherCommand = new AsyncRelayCommand(EditTeacherAsync, () => IsTeacherSelected && !IsBusy);
        DeleteTeacherCommand = new AsyncRelayCommand(DeleteTeacherAsync, () => CanDeleteTeacher && !IsBusy);

        Task.Run(async () => await LoadTeachersAsync());
    }

    private async Task LoadTeachersAsync()
    {
        try
        {
            IsBusy = true;
            var teachers = await _teacherService.GetAllTeachersAsync();
            var courses = await _courseService.GetAllCoursesAsync();

            Teachers.Clear();
            foreach (var teacher in teachers)
            {
                Teachers.Add(teacher);
            }

            Courses.Clear();
            foreach (var course in courses)
            {
                Courses.Add(course);
            }
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
        var isSaved = _windowService.OpenTeacherDialog(teacher, "Add Teacher");
        if (isSaved)
        {
            try
            {
                IsBusy = true;
                await _teacherService.AddTeacherAsync(teacher);
                Teachers.Add(teacher);
                _windowService.ShowMessageDialog("Teacher added successfully.", "Success");
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

        var teacherCopy = new Teacher
        {
            Id = SelectedTeacher.Id,
            FirstName = SelectedTeacher.FirstName,
            LastName = SelectedTeacher.LastName,
            Subject = SelectedTeacher.Subject,
            CourseId = SelectedTeacher.CourseId
        };

        var isSaved = _windowService.OpenTeacherDialog(teacherCopy, "Edit Teacher");
        if (isSaved)
        {
            try
            {
                IsBusy = true;
                await _teacherService.UpdateTeacherAsync(teacherCopy);

                SelectedTeacher.FirstName = teacherCopy.FirstName;
                SelectedTeacher.LastName = teacherCopy.LastName;
                SelectedTeacher.Subject = teacherCopy.Subject;
                SelectedTeacher.CourseId = teacherCopy.CourseId;
                SelectedTeacher.Course = await _courseService.GetCourseByIdAsync(teacherCopy.CourseId);

                OnPropertyChanged(nameof(Teachers));
                _windowService.ShowMessageDialog("Teacher updated successfully.", "Success");
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
                await _teacherService.DeleteTeacherAsync(SelectedTeacher.Id);
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
}
