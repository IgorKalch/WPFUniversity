using System;
using System.Windows;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.ViewModels.Courses;

namespace WpfUniversity.Views.Courses;

public partial class AddCourseForm : Window
{
    private IUnitOfWork _unitOfWork;

    public AddCourseForm(IUnitOfWork unitOfWork)
    {
        InitializeComponent();
        _unitOfWork = unitOfWork;

        DataContext = new AddCourseViewModel(_unitOfWork);
    }
}
