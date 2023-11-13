using System;
using System.Windows;
using UniversityDataLayer.Entities;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.Formses;
using WpfUniversity.Formses.Course;
using WpfUniversity.StartUpHelpers;
using WpfUniversity.ViewModels;

namespace WpfUniversity
{
    public partial class MainWindow : Window
    {
        private readonly IUnitOfWork _unit;
        private readonly MainWindowViewModel _viewModel;

        public MainWindow(IUnitOfWork unit, MainWindowViewModel viewModel) 
        {
            InitializeComponent();
            _unit = unit;
            _viewModel = viewModel;

            DataContext = _viewModel;
        }
        
        private void RefreshForm()
        {
            if (_viewModel != null)
            {
                _viewModel.UpdateCourses();
            }
        }

        private void coursesTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (coursesTree.SelectedItem is Student selectedItem)
            {
                //_courseForm.Create().Show();
            }
            else if (coursesTree.SelectedItem is Group selectedItem2)
            {
                //_groupForm.Create().Show();
            }
        }

        private void addCourse_Click(object sender, RoutedEventArgs e)
        {
            var form = new AddCourseForm(_unit);
            form.ShowDialog();

            RefreshForm();
        }

        private void editCourse_Click(object sender, RoutedEventArgs e)
        {
            if (coursesTree.SelectedItem is Course selectedCourse)
            {
                var form = new EditCourseForm(_unit, selectedCourse.Id);
                form.ShowDialog();

                RefreshForm();
            }
            else 
            {
                MessageBox.Show("Please select any course");
            }
        }

        private void deleteCourse_Click(object sender, RoutedEventArgs e)
        {
            if (coursesTree.SelectedItem is Course selectedCourse)
            {
                if(selectedCourse.Groups?.Count > 0)
                {
                    MessageBox.Show("You cannot delete this course because it has a group");
                }
                else
                {
                    var Result = MessageBox.Show("Are you sure you want to delete the Course?", "Deleting course", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (Result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _unit.CourseRepository.Remove(selectedCourse);
                            _unit.Commit();

                            MessageBox.Show("The course was deleted successfully");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            _unit.Dispose();
                        }
                    }
                }
               

                RefreshForm();
            }
            else
            {
                MessageBox.Show("Please select any course");
            }
        }
    }
}
