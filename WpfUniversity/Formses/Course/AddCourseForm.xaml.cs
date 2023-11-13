using System;
using System.Windows;
using UniversityDataLayer.Migrations;
using UniversityDataLayer.UnitOfWorks;

namespace WpfUniversity.Formses.Course;

public partial class AddCourseForm : Window
{
    private readonly IUnitOfWork _unit;

    public AddCourseForm(IUnitOfWork unit)
    {
        InitializeComponent();
        _unit = unit;
    }

    private void save_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(tbNameCourse.Text))
        {
            try
            {
                UniversityDataLayer.Entities.Course course = new UniversityDataLayer.Entities.Course();
                course.Name = tbNameCourse.Text;
                course.Description = tbDescriptionCourse.Text;

                _unit.CourseRepository.Add(course);
                _unit.Commit();

                MessageBox.Show("The course was recorded successfully");
                AddCourse_Form.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                _unit.Dispose();
            }
        }
        else
        {
            MessageBox.Show("Please enter the course name");
        }
    }

    private void close_Click(object sender, RoutedEventArgs e)
    {
        AddCourse_Form.Close();
    }
}
