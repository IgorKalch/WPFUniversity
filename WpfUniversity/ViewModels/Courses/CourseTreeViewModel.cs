using MvvmCross.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UniversityDataLayer.Entities;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;

namespace WpfUniversity.ViewModels.Courses
{
    public class CourseTreeViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unit;
        private ObservableCollection<Course> _courses = new();
        private readonly CourseService _courseService;
        private SelectedCourseService _selectedCourseService;
        private readonly ModalNavigationService _modalNavigationService;

        public ObservableCollection<Course> Courses => _courses;        

        public CourseTreeViewModel(IUnitOfWork unitOfWork, CourseService courseService, SelectedCourseService selectedCourseSErvice, ModalNavigationService modalNavigationService)
        {
            _unit = unitOfWork;
            _courseService = courseService;
            _selectedCourseService = selectedCourseSErvice;
            _modalNavigationService = modalNavigationService;

            if(_selectedCourseService != null)
                    _selectedCourseService.SelectedCourseChanged += SelectedCourseService_SelectedCourseChanged;


            _courseService.CourseLoaded += CourseService_CourseLoaded;
            _courseService.CourseAdded += CourseService_CourseAdded;
            _courseService.CourseUpdated += CourseService_Updated;
            _courseService.CourseDeleted += CourseService_CourseDeleted;

            Courses.CollectionChanged += Courses_CollectionChanged;
        }

        public Course SelectedCourse
        {
            get { return _courses.Where(y => y.Id == _selectedCourseService?.SelectedCourse.Id).FirstOrDefault(); }
            set {
                if (_selectedCourseService != null)
                {
                    _selectedCourseService.SelectedCourse = value;
                    RaisePropertyChanged(() => SelectedCourse);
                }
            }
        }

        private void SelectedCourseService_SelectedCourseChanged()
        {
            RaisePropertyChanged(() => SelectedCourse);
        }

        private void CourseService_CourseLoaded()
        {
            Courses.Clear();

            foreach (Course course in _courseService.Courses)
            {
                Courses.Add(course);
            }
        }

        private void CourseService_CourseAdded(Course course)
        {
            Courses.Add(course);
        }

        private void CourseService_Updated(Course course)
        {
           var newSelectedCourse = Courses.FirstOrDefault(y => y.Id == course.Id);

            if (SelectedCourse != newSelectedCourse)
            {
                SelectedCourse = newSelectedCourse;
            }
        }

        private void CourseService_CourseDeleted(Course course)
        {
            var courseToDelete = Courses.FirstOrDefault(y => y.Id == course.Id);

            if (courseToDelete != null)
            {
                Courses.Remove(courseToDelete);
            }
        }

        private void Courses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => SelectedCourse);
        }
    }
}
