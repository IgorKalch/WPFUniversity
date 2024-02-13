using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UniversityDataLayer.Entities;

namespace WpfUniversity.ViewModels.Groups;

public class GroupViewModel : INotifyPropertyChanged
{
    private readonly Group _group;

    public GroupViewModel(Group group)
    {
        _group = group;
    }
    public int CourseId
    {
        get { return _group.CourseId; }
        set
        {
            _group.CourseId = value;
            OnPropertyChanged(nameof(CourseId));
        }
    }

    public int TeacherId
    {
        get { return _group.TeacherId; }
        set
        {
            _group.TeacherId = value;
            OnPropertyChanged(nameof(TeacherId));
        }
    }

    public string? Name
    {
        get { return _group.Name; }
        set
        {
            _group.Name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public ObservableCollection<Student>? Students
    {
        get { return new ObservableCollection<Student>(_group.Students); }
        set
        {
            _group.Students = value.ToList();
            OnPropertyChanged(nameof(Students));
        }
    }

    public Course? Course
    {
        get { return _group.Course; }
        set
        {
            _group.Course = value;
            OnPropertyChanged(nameof(Course));
        }
    }

    public Teacher? Teacher
    {
        get { return _group.Techer; }
        set
        {
            _group.Techer = value;
            OnPropertyChanged(nameof(Teacher));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
    }
}
