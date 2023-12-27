using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using UniversityDataLayer.Entities;
using UniversityDataLayer.UnitOfWorks;

namespace WpfUniversity.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IUnitOfWork _unit;
    private Course _selectedCourse;

    public ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();

    public Course SelectedCourse
    {
        get { return _selectedCourse; }
        set
        {
            _selectedCourse = value;
           OnPropertyChanged(nameof(SelectedCourse));
        }
    }

    public MainWindowViewModel(IUnitOfWork unit)
    {
        _unit = unit;
        UpdateCourses();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
