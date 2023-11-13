using System;
using System.Windows;
using UniversityDataLayer.Entities;
using UniversityDataLayer.UnitOfWorks;

namespace WpfUniversity.Formses.Course;

public partial class EditCourseForm : Window
{
    private readonly IUnitOfWork _unit;
    private readonly int _courseId;

    private UniversityDataLayer.Entities.Course Course  => _unit.CourseRepository.GetById(_courseId);

    public EditCourseForm(IUnitOfWork unit ,int courseId)
    {
        InitializeComponent();
        _unit = unit;
        _courseId = courseId;

        Init();
    }

    private void Init()
    {
        if( Course != null)
        {
            tbNameCourse.Text = Course.Name;
            tbDescriptionCourse.Text = Course.Description;
        }        
    }

    private void save_Click(object sender, RoutedEventArgs e)
    {
        if (tbNameCourse.Text != Course.Name || tbDescriptionCourse.Text != Course.Description)
        {
            try
            {
                Course.Name = tbNameCourse.Text;
                Course.Description = tbDescriptionCourse.Text;

                _unit.CourseRepository.Update(Course);
                _unit.Commit();

                MessageBox.Show("The course was edited successfully");
                EditCourse_Form.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _unit.Dispose();
            }
        }
        else
        {
            MessageBox.Show("You have not changed anything");
        }
    }

    private void close_Click(object sender, RoutedEventArgs e)
    {
        EditCourse_Form.Close();
    }
}
