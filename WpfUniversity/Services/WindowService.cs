using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UniversityDataLayer.Entities;
using WpfUniversity.Services.Interfaces;
using WpfUniversity.ViewModels.Courses;
using WpfUniversity.ViewModels.Dialogs;
using WpfUniversity.ViewModels.Groups;
using WpfUniversity.ViewModels.Students;
using WpfUniversity.ViewModels.Teachers;
using WpfUniversity.Views.Courses;
using WpfUniversity.Views.Dialogs;
using WpfUniversity.Views.Groups;
using WpfUniversity.Views.Students;
using WpfUniversity.Views.Teachers;
using WpfUniversity.WindowFactories.Interfaces;

namespace WpfUniversity.Services;

public class WindowService : IWindowService
{
    private readonly IGroupsWindowFactory _groupsWindowFactory;
    private readonly IStudentsWindowFactory _studentsWindowFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITeacherViewModelFactory _teacherViewModelFactory;

    public WindowService(IGroupsWindowFactory groupsWindowFactory, IStudentsWindowFactory studentsWindowFactory, IServiceProvider serviceProvider, ITeacherViewModelFactory teacherViewModelFactory)
    {
        _groupsWindowFactory = groupsWindowFactory;
        _studentsWindowFactory = studentsWindowFactory;
        _serviceProvider = serviceProvider;
        _teacherViewModelFactory = teacherViewModelFactory;
    }

    public void OpenGroupsWindow(Course selectedCourse)
    {
        var groupsWindow = _groupsWindowFactory.Create(selectedCourse, this);
        groupsWindow.Show();
        groupsWindow.Focus();
    }

    public bool OpenTeacherDialog(Teacher teacher, string title)
    {
        var teacherViewModel = _teacherViewModelFactory.Create(this, teacher);
        //= new TeacherViewModel(_windowService, _teacherService, _courseService, teacher);

        var activeWindow = Application.Current.Windows
          .OfType<Window>()
          .FirstOrDefault(w => w.IsActive);

        var teacherEditWindow = new TeacherWindow
        {
            Title = title,
            DataContext = teacherViewModel,
            Owner = activeWindow
        };

        bool result = false;
        teacherViewModel.CloseRequested += (isSaved) =>
        {
            result = isSaved;
            teacherEditWindow.Close();
        };

        teacherEditWindow.ShowDialog();

        return result;
    }

    public void OpenTeachersWindow()
    {
        var courseWindow = _serviceProvider.GetRequiredService<TeachersWindow>();
        var viewModel = _serviceProvider.GetRequiredService<TeachersViewModel>();
        courseWindow.DataContext = viewModel;
        courseWindow.ShowDialog();
    }

    public void OpenAddCourseWindow()
    {
        var courseWindow = _serviceProvider.GetRequiredService<CourseWindow>();
        var viewModel = _serviceProvider.GetRequiredService<CourseViewModel>();
        viewModel.SetAddMode();
        courseWindow.DataContext = viewModel;
        courseWindow.ShowDialog();
    }

    public void OpenEditCourseWindow(Course course)
    {
        var courseWindow = _serviceProvider.GetRequiredService<CourseWindow>();
        var viewModel = _serviceProvider.GetRequiredService<CourseViewModel>();
        viewModel.SetCourse(course);
        courseWindow.DataContext = viewModel;
        courseWindow.ShowDialog();
    }

    public bool ShowConfirmationDialog(string message, string title)
    {
        var confirmationDialog = _serviceProvider.GetRequiredService<ConfirmationDialog>();
        var viewModel = _serviceProvider.GetRequiredService<ConfirmationDialogViewModel>();

        viewModel.Title = title;
        viewModel.Message = message;

        bool result = false;

        viewModel.OnConfirm = () =>
        {
            result = true;
            confirmationDialog.Close();
        };

        viewModel.OnCancel = () =>
        {
            result = false;
            confirmationDialog.Close();
        };

        var activeWindow = Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.IsActive);

        confirmationDialog.DataContext = viewModel;
        confirmationDialog.Owner = activeWindow;
        confirmationDialog.ShowDialog();

        return result;
    }

    public void ShowMessageDialog(string message, string title)
    {
        var errorDialog = _serviceProvider.GetRequiredService<ErrorDialog>();
        var viewModel = _serviceProvider.GetRequiredService<ErrorDialogViewModel>();

        viewModel.Title = title;
        viewModel.Message = message;

        viewModel.OnOk = () =>
        {
            errorDialog.Close();
        };

        var activeWindow = Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.IsActive);

        errorDialog.DataContext = viewModel;
        errorDialog.Owner = activeWindow;
        errorDialog.ShowDialog();
    }

    public void OpenAddGroupWindow(int courseId)
    {
        var groupWindow = _serviceProvider.GetRequiredService<GroupWindow>();
        var viewModel = _serviceProvider.GetRequiredService<GroupViewModel>();
        viewModel.SetAddMode(courseId);
        groupWindow.DataContext = viewModel;
        groupWindow.ShowDialog();
    }

    public void OpenEditGroupWindow(Group group)
    {
        var groupWindow = _serviceProvider.GetRequiredService<GroupWindow>();
        var viewModel = _serviceProvider.GetRequiredService<GroupViewModel>();
        viewModel.SetGroup(group);
        groupWindow.DataContext = viewModel;
        groupWindow.ShowDialog();
    }

    public void OpenStudentsWindow(Group group)
    {
        var studentsWindow = _studentsWindowFactory.Create(group, this);
        studentsWindow.Show();
        studentsWindow.Focus();
    }

    public void OpenAddStudentWindow(Group group)
    {
        var studentWindow = _serviceProvider.GetRequiredService<StudentWindow>();
        var viewModel = _serviceProvider.GetRequiredService<StudentViewModel>();
        viewModel.SetAddMode(group);
        studentWindow.DataContext = viewModel;
        studentWindow.ShowDialog();
    }

    public void OpenEditStudentWindow(Student student)
    {
        var studentWindow = _serviceProvider.GetRequiredService<StudentWindow>();
        var viewModel = _serviceProvider.GetRequiredService<StudentViewModel>();
        viewModel.SetStudent(student);
        studentWindow.DataContext = viewModel;
        studentWindow.ShowDialog();
    }

    public Task<string> ShowSaveFileDialogAsync(string filter, string title)
    {
        var tcs = new TaskCompletionSource<string>();

        Application.Current.Dispatcher.Invoke(() =>
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = filter,
                Title = title
            };
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
                tcs.SetResult(saveFileDialog.FileName);
            else
                tcs.SetResult(null);
        });

        return tcs.Task;
    }

    public Task<string> ShowOpenFileDialogAsync(string filter, string title)
    {
        var tcs = new TaskCompletionSource<string>();

        Application.Current.Dispatcher.Invoke(() =>
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = filter,
                Title = title
            };
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
                tcs.SetResult(openFileDialog.FileName);
            else
                tcs.SetResult(null);
        });

        return tcs.Task;
    }
}
