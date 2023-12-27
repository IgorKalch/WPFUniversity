using MvvmCross.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UniversityDataLayer.Entities;
using UniversityDataLayer.Migrations;
using UniversityDataLayer.UnitOfWorks;

namespace WpfUniversity.ViewModels.Courses;

public partial class CourseViewModel : MvxViewModel
{
    private readonly IUnitOfWork _unit;
    private ObservableCollection<Course> _courses = new();
    private ObservableCollection<Group> _groups = new();
    private ObservableCollection<Teacher> _teachers = new();
    private string _name;
    private string _description;
    private Course _selectedCourse;

    public CourseViewModel( IUnitOfWork unitOfWork)
    {
        _unit = unitOfWork;
        UpdateCourses();
    }

    public Course SelectedCourse
    {
        get { return _selectedCourse; }
        set { SetProperty(ref _selectedCourse, value); }
    }

    public ObservableCollection<Course> Courses 
    { 
        get { return _courses; }
        set { SetProperty(ref _courses, value); }
    }
    public string Name
    {
        get { return _name; }
        set
        {
            SetProperty( ref _name, value);
        }
    }

    public string Description
    {
        get { return _description; }
        set
        {
            SetProperty(ref _description, value);
        }
    }
    public ObservableCollection<Group> Groups
    {
        get { return _groups; }
        set
        {
            SetProperty(ref _groups, value);
        }
    }

    public ObservableCollection<Teacher> Teachers
    {
        get { return _teachers; }
        set
        {
            SetProperty(ref _teachers, value);
        }
    }

    public void UpdateCourses()
    {
        Courses.Clear();

        var coursesToAdd = _unit.CourseRepository.Get();

        foreach (var course in coursesToAdd)
        {
            var courseToadd = _unit.CourseRepository.GetById(course.Id);
            Courses.Add(courseToadd);
        }
    }
}
