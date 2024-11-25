using UniversityDataLayer.Entities;
using WpfUniversity.Services.Interfaces;
using WpfUniversity.ViewModels.Groups;
using WpfUniversity.WindowFactories.Interfaces;

namespace WpfUniversity.WindowFactories;

public class GroupsViewModelFactory : IGroupsViewModelFactory
{
    private readonly IGroupService _groupService;

    public GroupsViewModelFactory(IGroupService groupService)
    {
        _groupService = groupService;
    }

    public GroupsViewModel Create(Course selectedCourse)
    {
        return new GroupsViewModel(_groupService, selectedCourse);
    }
}
