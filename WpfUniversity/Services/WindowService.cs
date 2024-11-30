using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;
using UniversityDataLayer.Entities;
using WpfUniversity.Services.Interfaces;
using WpfUniversity.ViewModels.Courses;
using WpfUniversity.ViewModels.Dialogs;
using WpfUniversity.ViewModels.Groups;
using WpfUniversity.ViewModels.Students;
using WpfUniversity.Views.Courses;
using WpfUniversity.Views.Dialogs;
using WpfUniversity.Views.Groups;
using WpfUniversity.Views.Students;
using WpfUniversity.WindowFactories.Interfaces;

namespace WpfUniversity.Services;

public class WindowService : IWindowService
{
    private readonly IGroupsWindowFactory _groupsWindowFactory;
    private readonly IStudentsWindowFactory _studentsWindowFactory;
    private readonly IServiceProvider _serviceProvider;

    public WindowService(IGroupsWindowFactory groupsWindowFactory, IStudentsWindowFactory studentsWindowFactory, IServiceProvider serviceProvider)
    {
        _groupsWindowFactory = groupsWindowFactory;
        _studentsWindowFactory = studentsWindowFactory;
        _serviceProvider = serviceProvider;
    }

    public void OpenGroupsWindow(Course selectedCourse)
    {
        var groupsWindow = _groupsWindowFactory.Create(selectedCourse, this);
        groupsWindow.Show();
        groupsWindow.Focus();
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

    public void ShowErrorDialog(string message, string title)
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
}
