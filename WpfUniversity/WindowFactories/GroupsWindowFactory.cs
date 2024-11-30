using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using UniversityDataLayer.Entities;
using WpfUniversity.Services;
using WpfUniversity.ViewModels.Groups;
using WpfUniversity.Views.Groups;
using WpfUniversity.WindowFactories.Interfaces;

namespace WpfUniversity.WindowFactories;

public class GroupsWindowFactory : IGroupsWindowFactory
{
    private readonly IGroupsViewModelFactory _groupsViewModelFactory;
    private readonly IServiceProvider _serviceProvider;

    public GroupsWindowFactory(IGroupsViewModelFactory groupsViewModelFactory, IServiceProvider serviceProvider)
    {
        _groupsViewModelFactory = groupsViewModelFactory;
        _serviceProvider = serviceProvider;
    }

    public GroupsWindow Create(Course selectedCourse, WindowService windowService)
    {
        foreach (Window window in Application.Current.Windows)
        {
            if (window is GroupsWindow groupsWindow)
            {
                var viewModel = groupsWindow.DataContext as GroupsViewModel;
                if (viewModel != null && viewModel.Course.Id == selectedCourse.Id)
                {
                    groupsWindow.Activate();
                    return groupsWindow;
                }
            }
        }

        var newGroupsWindow = _serviceProvider.GetRequiredService<GroupsWindow>();
        var groupsViewModel = _groupsViewModelFactory.Create(selectedCourse, windowService);
        newGroupsWindow.DataContext = groupsViewModel;

        return newGroupsWindow;
    }
}
