using Microsoft.Extensions.DependencyInjection;
using System;
using UniversityDataLayer.Entities;
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

    public GroupsWindow Create(Course selectedCourse)
    {
        var groupsWindow = _serviceProvider.GetRequiredService<GroupsWindow>();

        var groupsViewModel = _groupsViewModelFactory.Create(selectedCourse);
        groupsWindow.DataContext = groupsViewModel;

        return groupsWindow;
    }
}
