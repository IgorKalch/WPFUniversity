using MvvmCross.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using UniversityDataLayer.Entities;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;
using WpfUniversity.Services.Groups;
using Group = UniversityDataLayer.Entities.Group;

namespace WpfUniversity.ViewModels.Courses;

public class CourseTreeViewModel : ViewModelBase
{
    #region Fields

    private ObservableCollection<Course> _courses = new();
    private readonly CourseService _courseService;
    private SelectedCourseService _selectedCourseService;
    private ObservableCollection<Group> _groups = new();
    private readonly GroupService _groupService;
    private readonly SelectedGroupService _selectedGroupService;
    private readonly ModalNavigationService _modalNavigationService;

    #endregion

    #region Constructor
    public CourseTreeViewModel(CourseService courseService, SelectedCourseService selectedCourseSErvice, GroupService groupService, SelectedGroupService selectedGroupService
        , ModalNavigationService modalNavigationService)
    {
        _courseService = courseService;
        _selectedCourseService = selectedCourseSErvice;
        _groupService = groupService;
        _selectedGroupService = selectedGroupService;
        _modalNavigationService = modalNavigationService;

        if(_selectedCourseService != null)
            _selectedCourseService.SelectedCourseChanged += SelectedCourseService_SelectedCourseChanged;

        if(_selectedGroupService != null)
            _selectedGroupService.SelectedGroupChanged += SelectedGroupService_SelectedGroupChanged;


        _courseService.CourseLoaded += CourseService_CourseLoaded;
        _courseService.CourseAdded += CourseService_CourseAdded;
        _courseService.CourseUpdated += CourseService_Updated;
        _courseService.CourseDeleted += CourseService_CourseDeleted;

        Courses.CollectionChanged += Courses_CollectionChanged;
       
        _groupService.GroupLoaded += GroupService_GroupLoaded;

        Groups.CollectionChanged += Groups_CollectionChanged;


    }

    #endregion

    protected override void Dispose()
    {
        _selectedCourseService.SelectedCourseChanged -= SelectedCourseService_SelectedCourseChanged;
        _selectedGroupService.SelectedGroupChanged -= SelectedGroupService_SelectedGroupChanged;

        _courseService.CourseLoaded -= CourseService_CourseLoaded;
        _courseService.CourseAdded -= CourseService_CourseAdded;
        _courseService.CourseUpdated -= CourseService_Updated;
        _courseService.CourseDeleted -= CourseService_CourseDeleted;

        base.Dispose();
    }

    #region Public Properties

    public ObservableCollection<Course> Courses => _courses;
    public ObservableCollection<Group> Groups => _groups;

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

    public Group SelectedGroup
    {
        get {
            var a = _groups.FirstOrDefault(g => g.Id == _selectedGroupService?.SelectedGroup?.Id);
            return  a; 
        }
        set
        {
            if (_selectedGroupService != null)
            {
                _selectedGroupService.SelectedGroup = value;
                OnPropertyChanged(nameof(SelectedCourse));
                OnPropertyChanged(nameof(SelectedGroup));
                OnPropertyChanged(nameof(IsSelectedGroup));
            }
        }
    }

    public bool IsSelectedGroup => SelectedGroup != null;

    public bool CanDeleteCourse => SelectedCourse != null && (SelectedCourse.Groups?.Count ?? 0) == 0;

    public bool CanEditCourse => SelectedCourse != null;

    #endregion

    #region Course Private metods
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

        GroupService_GroupLoaded();
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

    #endregion


    #region Groups Private metods

    private void SelectedGroupService_SelectedGroupChanged()
    {
        OnPropertyChanged(nameof(SelectedGroup));
    }

    private void GroupService_GroupLoaded()
    {
        Groups.Clear();

        if (_selectedCourseService != null && _selectedCourseService.SelectedCourse != null &&  _selectedCourseService.SelectedCourse.Groups != null)
        {
            foreach (Group group in _selectedCourseService!.SelectedCourse!.Groups)
            {
                Groups.Add(group);
            }
        }       
    }

    private void Groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(SelectedGroup));
    }

    #endregion
}



