using System;
using System.Threading.Tasks;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;
using WpfUniversity.ViewModels.Courses;
using WpfUniversity.ViewModels.Dialogs;

namespace WpfUniversity.Command.Courses;

public class DeleteCourseCommand : AsyncCommandBase
{
    private readonly CourseViewModel _courseViewModel;
    private readonly CourseService _courseService;
    private readonly ModalNavigationService _modalNavigationService;

    public DeleteCourseCommand(CourseViewModel courseViewModel, CourseService courseService, ModalNavigationService modalNavigationService)
    {
        _courseViewModel = courseViewModel;
        _courseService = courseService;
        _modalNavigationService = modalNavigationService;
    }

    public override async Task ExecuteAsync(object parameter)
    {
        // todo: Messange.Show in ViewModel, is it a good idea?
        /*
        YesNoDialogViewModel dialog = new YesNoDialogViewModel(_modalNavigationService);
        dialog.Message = "";        
        dialog.ShowDialogCommand.Execute(null);
        if (!dialog.UserChoice) return; 
        */

        _courseViewModel.ErrorMessage = null;
        var course = _courseViewModel.CourseTreeViewModel.SelectedCourse;

        try
        {
            await _courseService.Delete(course);
        }
        catch (Exception e)
        {
            _courseViewModel.ErrorMessage = e.Message;
        }

    }
}
