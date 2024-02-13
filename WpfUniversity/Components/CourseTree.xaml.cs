using System.Windows;
using System.Windows.Controls;
using UniversityDataLayer.Entities;
using WpfUniversity.ViewModels.Courses;

namespace WpfUniversity.Components;

public partial class CourseTree : UserControl
{
    public CourseTree()
    {
        InitializeComponent();
    }

    private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (DataContext is CourseTreeViewModel viewModel)
        {
            viewModel.SelectedCourse = e.NewValue as Course;
        }
    }
}
