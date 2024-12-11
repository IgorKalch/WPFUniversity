using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using UniversityDataLayer.Entities;
using WpfUniversity.Services.Interfaces;
using WpfUniversity.ViewModels.Students;
using WpfUniversity.Views.Students;
using WpfUniversity.WindowFactories.Interfaces;

namespace WpfUniversity.WindowFactories;

public class StudentsWindowFactory : IStudentsWindowFactory
{

    private readonly IStudentsViewModelFactory _studentsViewModelFactory;
    private readonly IServiceProvider _serviceProvider;

    public StudentsWindowFactory(IStudentsViewModelFactory studentsViewModelFactory, IServiceProvider serviceProvider)
    {
        _studentsViewModelFactory = studentsViewModelFactory;
        _serviceProvider = serviceProvider;
    }

    public StudentsWindow Create(Group selectedGroup, IWindowService windowService)
    {
        foreach (Window window in Application.Current.Windows)
        {
            if (window is StudentsWindow studentsWindow)
            {
                var viewModel = studentsWindow.DataContext as StudentsViewModel;
                if (viewModel != null && viewModel.Group.Id == selectedGroup.Id)
                {
                    studentsWindow.Activate();
                    return studentsWindow;
                }
            }
        }

        var newStudentsWindow = _serviceProvider.GetRequiredService<StudentsWindow>();
        var studentsViewModel = _studentsViewModelFactory.Create(selectedGroup, windowService);
        newStudentsWindow.DataContext = studentsViewModel;

        return newStudentsWindow;
    }
}
