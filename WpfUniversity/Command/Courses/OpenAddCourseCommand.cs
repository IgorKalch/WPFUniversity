using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUniversity.Services.Courses;
using WpfUniversity.Services;
using WpfUniversity.ViewModels.Courses;
using System.Windows.Input;

namespace WpfUniversity.Command.Courses
{
    public class OpenAddCourseCommand : CommandBase
    {
        private readonly CourseService _courseService;
        private readonly ModalNavigationService _modalNavigationService;

        public OpenAddCourseCommand(CourseService courseService, ModalNavigationService modalNavigationService) 
        {
            _courseService = courseService;
            _modalNavigationService = modalNavigationService;
        }

        public override void Execute(object parameter)
        {
            AddCourseViewModel model = new AddCourseViewModel(_courseService, _modalNavigationService);
            _modalNavigationService.CurrentViewModel = model;
        }
    }
}
