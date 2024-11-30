using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UniversityDataLayer.Entities;
using WpfUniversity.Commands;
using WpfUniversity.Services.Interfaces;

namespace WpfUniversity.ViewModels.Students;

public class StudentViewModel : ViewModelBase
{
    private readonly IStudentService _studentService;
    private readonly IWindowService _windowService;

    public StudentViewModel(IStudentService studentService, IWindowService windowService)
    {
        _studentService = studentService;
        _windowService = windowService;

        SaveCommand = new AsyncRelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    public string WindowTitle { get; set; }

    private string _firstName;
    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    private string _secondName;
    public string LastName
    {
        get => _secondName;
        set => SetProperty(ref _secondName, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public bool IsSaved { get; private set; }

    private Group _group;
    private Student _student;
    public bool IsEditMode { get; private set; }

    public void SetAddMode(Group group)
    {
        _group = group;
        IsEditMode = false;
        WindowTitle = "Add Student";
        OnPropertyChanged(nameof(WindowTitle));
    }

    public void SetStudent(Student student)
    {
        _student = student;
        IsEditMode = true;
        WindowTitle = "Edit Student";
        FirstName = student.FirstName;
        LastName = student.LastName;
        OnPropertyChanged(nameof(WindowTitle));
    }

    private async Task Save()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                _windowService.ShowErrorDialog("Full Name is required.", "Error");
                return;
            }

            if (IsEditMode)
            {
                _student.FirstName = FirstName;
                _student.LastName = LastName;
                _studentService.Update(_student);
            }
            else
            {
                var newStudent = new Student
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    GroupId = _group.Id
                };
                _studentService.Add(newStudent);
            }

            IsSaved = true;
            CloseWindow();
        }
        catch (Exception ex)
        {
            _windowService.ShowErrorDialog($"{ex.Message}", "Error");
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
}