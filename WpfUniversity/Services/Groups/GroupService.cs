using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.Services.Interfaces;

namespace WpfUniversity.Services.Courses;

public class GroupService : IGroupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly List<Group> _groups = [];
    public List<Group> Groups => _groups;


    public GroupService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task LoadGroupsByCourseId(int courseId)
    {
        Groups.Clear();

        var groupsToAdd = _unitOfWork.GroupRepository.Get(x => x.CourseId == courseId);

        foreach (var group in groupsToAdd)
        {
            var groupToAdd = _unitOfWork.GroupRepository.GetById(group.Id);
            Groups.Add(groupToAdd);
        }
    }

    public async Task Add(Group group)
    {
        _unitOfWork.GroupRepository.Add(group);
        _unitOfWork.Commit();
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
    }
}
