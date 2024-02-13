using WpfUniversity.Services.Courses;
using WpfUniversity.Services;
using WpfUniversity.ViewModels.Courses;

namespace WpfUniversity.Command.Courses;

public class OpenEditCourseCommand : CommandBase
{
    private readonly CourseTreeViewModel _courseTreeViewModel;
    private readonly CourseService _courseService;
    private readonly ModalNavigationService _modalNavigationService;

    public OpenEditCourseCommand(CourseTreeViewModel courseTreeViewModel, CourseService courseService, ModalNavigationService modalNavigationService) 
    {
        _courseTreeViewModel = courseTreeViewModel;
        _courseService = courseService;
        _modalNavigationService = modalNavigationService;
    }

    public override void Execute(object parameter)
    {
        EditCourseViewModel model = new EditCourseViewModel(_courseTreeViewModel, _courseService, _modalNavigationService);
        _modalNavigationService.CurrentViewModel = model;
    }
}
