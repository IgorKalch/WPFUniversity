using System;
using UniversityDataLayer.Entities;
using WpfUniversity.Services.Courses;

namespace WpfUniversity.Services.Groups;

public class SelectedGroupService
{
    private readonly GroupService _groupService;
    private Group _selectedGroup;

    public Group SelectedGroup
    {
        get{ return _selectedGroup; }
        set
        {
            _selectedGroup = value;
            SelectedGroupChanged?.Invoke();
        }
    }

    public event Action SelectedGroupChanged;

    public SelectedGroupService(GroupService groupService)
    {
        _groupService = groupService;

        _groupService.GroupAdded += GroupService_GroupAdded;
        _groupService.GroupUpdated += GroupService_GroupUpdated;
    }

    private void GroupService_GroupAdded(Group group)
    {
        SelectedGroup = group;
    }

    private void GroupService_GroupUpdated(Group group)
    {
        if (group.Id == SelectedGroup?.Id)
        {
            SelectedGroup = group;
        }
    }
}
