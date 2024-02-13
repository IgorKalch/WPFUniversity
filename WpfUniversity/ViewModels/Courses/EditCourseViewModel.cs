using System.Windows.Input;
using WpfUniversity.Command;
using WpfUniversity.Command.Courses;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;

namespace WpfUniversity.ViewModels.Courses;

public class EditCourseViewModel : ViewModelBase
{
    public int Id { get;}
    public EditCourseFormViewModel EditCourseFormViewModel { get; }

    public EditCourseViewModel(CourseTreeViewModel courseTreeViewModel, CourseService courseService, ModalNavigationService modalNavigationService)
    {
        Id = courseTreeViewModel.SelectedCourse.Id;
        ICommand submitCommand = new EditCourseCommand(this,courseService,modalNavigationService);
        ICommand cancelCommand = new CloseModalCommand(modalNavigationService);

        EditCourseFormViewModel = new EditCourseFormViewModel(submitCommand, cancelCommand)
        {            
            Name = courseTreeViewModel.SelectedCourse.Name,
            Description = courseTreeViewModel.SelectedCourse.Description
        };
    }   
}
