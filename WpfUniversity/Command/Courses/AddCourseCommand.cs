using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;
using WpfUniversity.ViewModels.Courses;

namespace WpfUniversity.Command.Courses
{
    public class AddCourseCommand : AsyncCommandBase
    {
        private readonly AddCourseViewModel _courseViewModel;
        private readonly CourseService _courseService;
        private readonly ModalNavigationService _modalNavigationService;

        public AddCourseCommand(AddCourseViewModel courseViewModel, CourseService courseService, ModalNavigationService modalNavigationService)
        {
            _courseViewModel = courseViewModel;
            _courseService = courseService;
            _modalNavigationService = modalNavigationService;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            AddCourseFormViewModel viewModel = _courseViewModel.AddCourseFormViewModel;
            viewModel.ErrorMessage = null;

            Course course = new Course();
            course.Name = viewModel.Name;
            course.Description = viewModel.Description;

            try
            {
                await _courseService.Add(course);

                _modalNavigationService.Close();
            }
            catch (Exception)
            {
                viewModel.ErrorMessage = "Failed to add Course. Please try again later.";
            }            
        }
    }
}
