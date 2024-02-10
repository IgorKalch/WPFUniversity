using MvvmCross.Commands;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using UniversityDataLayer.Entities;
using WpfUniversity.Command;
using WpfUniversity.Command.Courses;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;

namespace WpfUniversity.ViewModels.Courses;

public class AddCourseViewModel : ViewModelBase
{
    public AddCourseFormViewModel AddCourseFormViewModel { get; }

    public AddCourseViewModel(CourseService courseService, ModalNavigationService modalNavigationService)
    {
        ICommand submitCommand = new AddCourseCommand(this,courseService,modalNavigationService);
        ICommand cancelCommand = new CloseModalCommand(modalNavigationService);

        AddCourseFormViewModel = new AddCourseFormViewModel(submitCommand, cancelCommand);

    }   
}
