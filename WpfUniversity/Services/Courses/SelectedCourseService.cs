using System;
using UniversityDataLayer.Entities;

namespace WpfUniversity.Services.Courses;

public class SelectedCourseService
{
    private readonly CourseService _courseService;
    private Course _selectedCourse;

    public Course SelectedCourse
    {
        get{ return _selectedCourse;}
        set
        {
            _selectedCourse = value;
            SelectedCourseChanged?.Invoke();
        }
    }

    public event Action SelectedCourseChanged;

    public SelectedCourseService(CourseService courseService)
    {
        _courseService = courseService;

        _courseService.CourseAdded += CourseService_CourseAdded;
        _courseService.CourseUpdated += CourseService_CourseUpdated;
    }

    private void CourseService_CourseAdded(Course course)
    {
        SelectedCourse = course;
    }

    private void CourseService_CourseUpdated(Course course)
    {
        if (course.Id == SelectedCourse?.Id)
        {
            SelectedCourse = course;
        }
    }
}
