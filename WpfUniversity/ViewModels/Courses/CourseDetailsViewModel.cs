using MvvmCross.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.Services.Courses;

namespace WpfUniversity.ViewModels.Courses
{
    public class CourseDetailsViewModel : ViewModelBase
    {
        private SelectedCourseService _selectedCourse;       
        private readonly IUnitOfWork _unit;

        public Course SelectedCourse => _selectedCourse.SelectedCourse;

        public CourseDetailsViewModel(SelectedCourseService selectedCourse)
        {
            _selectedCourse = selectedCourse;
            _selectedCourse.SelectedCourseChanged += SelectedCourseService_SelectedCourseChanged;
        }


        public IMvxCommand EditCourseCommand { get; }
        public IMvxCommand DeleteCourseCommand { get; }

        private void SelectedCourseService_SelectedCourseChanged()
        {
            RaisePropertyChanged(() => SelectedCourse);
        }
    }
}
