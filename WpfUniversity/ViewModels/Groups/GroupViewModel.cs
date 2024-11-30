using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UniversityDataLayer.Entities;
using WpfUniversity.Commands;
using WpfUniversity.Services.Interfaces;

namespace WpfUniversity.ViewModels.Groups;

public class GroupViewModel : ViewModelBase
{
    private readonly IGroupService _groupService;
    private readonly ITeacherService _teacherService;
    private readonly IWindowService _windowService;

    public GroupViewModel(IGroupService groupService, IWindowService windowService, ITeacherService teacherService)
    {
        _groupService = groupService;
        _windowService = windowService;
        _teacherService = teacherService;

        LoadTeachersCommand = new AsyncRelayCommand(LoadTeachersAsync);
        SaveCommand = new AsyncRelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    public string WindowTitle { get; set; }
    public ObservableCollection<Teacher> Teachers { get; set; } = new ObservableCollection<Teacher>();

    private Teacher _selectedTeacher;

    public Teacher SelectedTeacher
    {
        get => _selectedTeacher;
        set => SetProperty(ref _selectedTeacher, value);
    }

    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private int _teacherId;
    public int TeacherId
    {
        get => _teacherId;
        set => SetProperty(ref _teacherId, value);
    }

    private int _courseId;
    public int CourseId
    {
        get => _courseId;
        set => SetProperty(ref _courseId, value);
    }

    public ICommand LoadTeachersCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public bool IsSaved { get; private set; }

    private Group _group;
    public bool IsEditMode { get; private set; }

    public void SetGroup(Group group)
    {
        _group = group;
        IsEditMode = true;

        Name = group.Name;
        CourseId = group.CourseId;
        TeacherId = group.TeacherId;

        WindowTitle = "Edit Group";
        OnPropertyChanged(nameof(WindowTitle));
        LoadTeachersCommand.Execute(this);
    }

    public void SetAddMode(int courseId)
    {
        IsEditMode = false;
        WindowTitle = "Add Group";
        CourseId = courseId;
        OnPropertyChanged(nameof(WindowTitle));
        LoadTeachersCommand.Execute(this);
    }
    private async Task LoadTeachersAsync()
    {
        var teachers = await _teacherService.GetAllTeachersByCourseIdAsync(CourseId);

        Teachers.Clear();
        foreach (var teacher in teachers)
        {
            Teachers.Add(teacher);
        }
    }

    private async Task Save()
    {
        try
        {

            if (string.IsNullOrWhiteSpace(Name))
            {
                _windowService.ShowErrorDialog("Name is required.", "Error");
                return;
            }

            if (SelectedTeacher is null)
            {
                _windowService.ShowErrorDialog("Teacher is required.", "Error");
                return;
            }

            bool isUnique = await _groupService.IsGroupNameUniqueAsync(Name, IsEditMode ? (int?)_group.Id : null);
            if (!isUnique)
            {
                _windowService.ShowErrorDialog("A group with this name already exists. Please choose a different name.", "Validation Error");
                return;
            }

            if (IsEditMode)
            {
                _group.Name = Name;
                _group.CourseId = CourseId;
                _group.TeacherId = SelectedTeacher.Id;

                _groupService.Update(_group);
            }
            else
            {
                var newGroup = new Group
                {
                    Name = Name,
                    CourseId = CourseId,
                    TeacherId = SelectedTeacher.Id,
                };
                await _groupService.Add(newGroup);
            }

            IsSaved = true;

            CloseWindow();
        }
        catch (Exception ex)
        {
            string userFriendlyMessage = $"An unexpected error occurred: {GetExceptionMessages(ex)}";

            _windowService.ShowErrorDialog(userFriendlyMessage, "Error");
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
    private string GetExceptionMessages(Exception ex)
    {
        if (ex == null)
            return string.Empty;

        string message = ex.Message;
        if (ex.InnerException != null)
        {
            message += " --> " + GetExceptionMessages(ex.InnerException);
        }
        return message;
    }
}