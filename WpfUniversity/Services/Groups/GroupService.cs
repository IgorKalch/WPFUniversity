using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;
using UniversityDataLayer.UnitOfWorks;

namespace WpfUniversity.Services.Courses;

public class GroupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly List<Group> _groups = new();
    public List<Group> Groups => _groups;

    public event Action GroupLoaded;
    public event Action<Group> GroupAdded;
    public event Action<Group> GroupUpdated;
    public event Action<Group> GroupDeleted;

    public GroupService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Load()
    {
        Groups.Clear();

        var groupsToAdd = _unitOfWork.GroupRepository.Get();

        foreach (var group in groupsToAdd)
        {
            var groupToAdd = _unitOfWork.GroupRepository.GetById(group.Id);
            Groups.Add(groupToAdd);
        }

        GroupLoaded?.Invoke();
    }

    public async Task Add(Group group)
    {
        _unitOfWork.GroupRepository.Add(group);
        _unitOfWork.Commit();

        GroupAdded?.Invoke(group);
    }

    public async Task Update(Group group)
    {
        var groupToUpdate = _unitOfWork.GroupRepository.GetById(group.Id);

        if (groupToUpdate != null)
        {
            groupToUpdate.Name = group.Name;
            groupToUpdate.TeacherId = group.TeacherId;
            groupToUpdate.CourseId = group.CourseId;

            _unitOfWork.GroupRepository.Update(groupToUpdate);
            _unitOfWork.Commit();

            GroupUpdated?.Invoke(groupToUpdate);
        }
        else
        {
            throw new Exception("Group not found.");
        }
    }

    public async Task Delete(Group group)
    {
        _unitOfWork.GroupRepository.Remove(group);
        _unitOfWork.Commit();

        GroupDeleted?.Invoke(group);
    }
}
