using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUniversity.Services.Courses;
using WpfUniversity.ViewModels.Courses;

namespace WpfUniversity.Command.Courses
{
    public class LoadCourseCommand : AsyncCommandBase
    {
        private readonly CourseViewModel _courseViewModel;
        private readonly CourseService _courseService;

        public LoadCourseCommand(CourseViewModel courseViewModel, CourseService courseService)
        {
            _courseViewModel = courseViewModel;
            _courseService = courseService;
        }

        public override async Task ExecuteAsync(object parameter)
        {

            _courseViewModel.ErrorMessage = null;
            _courseViewModel.IsLoading = true;

            try
            {
                await _courseService.Load();
            }
            catch (Exception)
            {
                _courseViewModel.ErrorMessage = "Failed to load Course. Please restart the application.";
            }
            
        }
    }
}
