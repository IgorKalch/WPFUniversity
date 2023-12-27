using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using UniversityDataLayer.Entities;
using UniversityDataLayer.Migrations;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.Views.Courses;

namespace WpfUniversity.ViewModels.Courses;

public class AddCourseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<Course> Phones { get; set; }

    // команда добавления нового объекта
    private RelayCommand addCommand;
    private Course selectedPhone;
    private readonly IUnitOfWork _unitOfWork;

    public RelayCommand AddCommand
    {
        get
        {
            return addCommand ??
              (addCommand = new RelayCommand(obj =>
              {
                  Course phone = new Course();
                  Phones.Insert(0, phone);
                  SelectedPhone = phone;
              }));
        }
    }

    public Course SelectedPhone
    {
        get { return selectedPhone; }
        set
        {
            selectedPhone = value;
            OnPropertyChanged("SelectedPhone");
        }
    }

    public AddCourseViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        Phones = new ObservableCollection<Course>(_unitOfWork.CourseRepository.Get());
     
    }

    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
    }
}
