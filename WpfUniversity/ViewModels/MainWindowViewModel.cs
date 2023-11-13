using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using UniversityDataLayer.Entities;
using UniversityDataLayer.UnitOfWorks;

namespace WpfUniversity.ViewModels;

public partial class MainWindowViewModel : ObservableObject 
{
    private readonly IUnitOfWork _unit;

    [ObservableProperty]
    ObservableCollection<Course> courses = new ObservableCollection<Course>();

    public MainWindowViewModel(IUnitOfWork unit)
    {
        _unit = unit;
        UpdateCourses();
    }

    public void UpdateCourses()
    {
        courses.Clear();

        var coursesToAdd = _unit.CourseRepository.Get();

        foreach (var course in coursesToAdd)
        {
            var courseToadd = _unit.CourseRepository.GetById(course.Id);
            courses.Add(courseToadd);
        }
    }

}
