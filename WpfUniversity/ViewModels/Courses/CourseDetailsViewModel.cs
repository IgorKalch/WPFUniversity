using MvvmCross.Commands;
using UniversityDataLayer.Entities;
using WpfUniversity.Services.Courses;

namespace WpfUniversity.ViewModels.Courses
{
    public class CourseDetailsViewModel : ViewModelBase
    {
        private SelectedCourseService _selectedCourse;  
        public Course SelectedCourse => _selectedCourse.SelectedCourse;

        public CourseDetailsViewModel(SelectedCourseService selectedCourse)
        {
            _selectedCourse = selectedCourse;
            _selectedCourse.SelectedCourseChanged += SelectedCourseService_SelectedCourseChanged;
        }

        protected override void Dispose()
        {
            _selectedCourse.SelectedCourseChanged -= SelectedCourseService_SelectedCourseChanged;

            base.Dispose();
        }

        public IMvxCommand EditCourseCommand { get; }
        public IMvxCommand DeleteCourseCommand { get; }

        private void SelectedCourseService_SelectedCourseChanged()
        {
            OnPropertyChanged(nameof(SelectedCourse));
        }
    }
}
