using UniversityDataLayer.Entities;
using WpfUniversity.Services.Interfaces;
using WpfUniversity.ViewModels.Groups;
using WpfUniversity.WindowFactories.Interfaces;

namespace WpfUniversity.WindowFactories;

public class GroupsViewModelFactory : IGroupsViewModelFactory
{
    private readonly IGroupService _groupService;
    private readonly ITeacherService _teacherService;

    public GroupsViewModelFactory(IGroupService groupService, ITeacherService teacherService)
    {
        _groupService = groupService;
        _teacherService = teacherService;
    }

    public GroupsViewModel Create(Course selectedCourse, IWindowService windowService)
    {
        var groupsViewModel = new GroupsViewModel(_groupService, windowService, _teacherService);
        groupsViewModel.Initialize(selectedCourse);

        return groupsViewModel;
    }
}
