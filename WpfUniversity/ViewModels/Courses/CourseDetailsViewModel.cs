using MvvmCross.Commands;
using UniversityDataLayer.Entities;
using WpfUniversity.Services.Courses;

namespace WpfUniversity.ViewModels.Courses
{
    public class CourseDetailsViewModel : ViewModelBase
    {
        private SelectedCourseService _selectedCourse;
        private string? _errorMessage;
        private bool _hasErrorMessage;

        public Course SelectedCourse => _selectedCourse.SelectedCourse;

        public string? ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(HasErrorMessage));
            }
        }
        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

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

        private void SelectedCourseService_SelectedCourseChanged()
        {
            OnPropertyChanged(nameof(SelectedCourse));
        }
    }
}
