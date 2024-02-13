using MvvmCross.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UniversityDataLayer.Entities;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;

namespace WpfUniversity.ViewModels.Courses;

public class CourseTreeViewModel : ViewModelBase
{
    private ObservableCollection<Course> _courses = new();
    private readonly CourseService _courseService;
    private SelectedCourseService _selectedCourseService;
    private readonly ModalNavigationService _modalNavigationService;

    public ObservableCollection<Course> Courses => _courses;        

    public CourseTreeViewModel(CourseService courseService, SelectedCourseService selectedCourseSErvice, ModalNavigationService modalNavigationService)
    {
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

    protected override void Dispose()
    {
        _selectedCourseService.SelectedCourseChanged -= SelectedCourseService_SelectedCourseChanged;

        _courseService.CourseLoaded -= CourseService_CourseLoaded;
        _courseService.CourseAdded -= CourseService_CourseAdded;
        _courseService.CourseUpdated -= CourseService_Updated;
        _courseService.CourseDeleted -= CourseService_CourseDeleted;

        base.Dispose();
    }

    public Course SelectedCourse
    {
        get { return _courses.FirstOrDefault(y => y.Id == _selectedCourseService?.SelectedCourse?.Id); }
        set {
            if (_selectedCourseService != null)
            {
                _selectedCourseService.SelectedCourse = value;
                OnPropertyChanged(nameof(SelectedCourse));
                OnPropertyChanged(nameof(CanDeleteCourse));
                OnPropertyChanged(nameof(CanEditCourse));
            }
        }
    }

    public bool CanDeleteCourse => SelectedCourse != null && (SelectedCourse.Groups?.Count ?? 0) == 0;

    public bool CanEditCourse => SelectedCourse != null;

    private void SelectedCourseService_SelectedCourseChanged()
    {
        OnPropertyChanged(nameof(SelectedCourse));
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
       var newSelectedCourse = _courseService.Courses.FirstOrDefault(y => y.Id == course.Id);

        if (newSelectedCourse != null)
        {
            _selectedCourseService.SelectedCourse = course;
        }
        CourseService_CourseLoaded();
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
        OnPropertyChanged(nameof(SelectedCourse));
    }
}
