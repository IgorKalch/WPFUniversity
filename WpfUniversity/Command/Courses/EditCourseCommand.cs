using System;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;
using WpfUniversity.ViewModels.Courses;

namespace WpfUniversity.Command.Courses;

public class EditCourseCommand : AsyncCommandBase
{
    private readonly EditCourseViewModel _courseViewModel;
    private readonly CourseService _courseService;
    private readonly ModalNavigationService _modalNavigationService;

    public EditCourseCommand(EditCourseViewModel courseViewModel, CourseService courseService, ModalNavigationService modalNavigationService)
    {
        _courseViewModel = courseViewModel;
        _courseService = courseService;
        _modalNavigationService = modalNavigationService;
    }

    public override async Task ExecuteAsync(object parameter)
    {
        EditCourseFormViewModel viewModel = _courseViewModel.EditCourseFormViewModel;
        viewModel.ErrorMessage = null;
        viewModel.IsSubmitting = true;

        Course course = new Course();
        course.Id = _courseViewModel.Id;
        course.Name = viewModel.Name;
        course.Description = viewModel.Description;

        try
        {
            await _courseService.Update(course);

            _modalNavigationService.Close();
        }
        catch (Exception e)
        {
            viewModel.ErrorMessage = e.Message;
        }
        finally
        {
            viewModel.IsSubmitting = false;
        }

    }
}
