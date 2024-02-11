using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;
using WpfUniversity.ViewModels.Courses;
using WpfUniversity.ViewModels.Dialogs;

namespace WpfUniversity.Command.Courses
{
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
            // todo: This show method yes/no dialog doesn't work
            /*
            YesNoDialogViewModel dialog = new YesNoDialogViewModel(_modalNavigationService);
            dialog.Message = "Do you want to proceed?";

            _modalNavigationService.CurrentViewModel = dialog;
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
}
