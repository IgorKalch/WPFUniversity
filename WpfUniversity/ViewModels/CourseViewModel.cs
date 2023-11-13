using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using UniversityDataLayer.Entities;

namespace WpfUniversity.ViewModels;

public partial class CourseViewModel : ObservableObject
{
    [ObservableProperty]
    ObservableCollection<Course> courses;
}
