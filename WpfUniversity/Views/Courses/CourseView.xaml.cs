using MvvmCross.Platforms.Wpf.Views;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.ViewModels.Courses;

namespace WpfUniversity.Views.Courses;

public partial class CourseView : MvxWpfView
{
    private readonly IUnitOfWork _unit;

    public CourseView(IUnitOfWork unit)
    {
        InitializeComponent();
        _unit = unit;

        DataContext = new CourseViewModel(_unit);
    }
}
