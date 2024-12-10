using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UniversityDataLayer.Entities;
using WpfUniversity.Commands;
using WpfUniversity.Services.Interfaces;

namespace WpfUniversity.ViewModels.Courses;

public class CourseViewModel : ViewModelBase
{
    private readonly ICourseService _courseService;
    private readonly IWindowService _windowService;

    public CourseViewModel(ICourseService courseService, IWindowService windowService)
    {
        _courseService = courseService;
        _windowService = windowService;

        SaveCommand = new AsyncRelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    public string WindowTitle { get; set; }

    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _description;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public bool IsSaved { get; private set; }

    private async Task Save()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                _windowService.ShowMessageDialog("Name is required.", "Error");
                return;
            }

            if (IsEditMode)
            {
                _course.Name = Name;
                _course.Description = Description;

                _courseService.Update(_course);
            }
            else
            {
                var newCourse = new Course
                {
                    Name = Name,
                    Description = Description
                };
                _courseService.Add(newCourse);
            }

            IsSaved = true;

            CloseWindow();
        }
        catch (Exception ex)
        {
            _windowService.ShowMessageDialog($"{ex.Message}", "Error");
        }
    }

    private void Cancel()
    {
        IsSaved = false;
        CloseWindow();
    }

    private void CloseWindow()
    {
        if (Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this) is Window window)
        {
            window.Close();
        }
    }

    private Course _course;
    public bool IsEditMode { get; private set; }

    public void SetCourse(Course course)
    {
        _course = course;
        IsEditMode = true;

        Name = course.Name;
        Description = course.Description;

        WindowTitle = "Edit Course";
        OnPropertyChanged(nameof(WindowTitle));
    }

    public void SetAddMode()
    {
        IsEditMode = false;
        WindowTitle = "Add Course";
        OnPropertyChanged(nameof(WindowTitle));
    }
}
