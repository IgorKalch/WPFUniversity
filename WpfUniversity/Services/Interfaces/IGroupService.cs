using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;

namespace WpfUniversity.Services.Interfaces;

public interface IGroupService
{
    List<Group> Groups { get; }
    Task Add(Group group);
    Task Update(Group group);
    Task Delete(Group group);
    Task LoadGroupsByCourseId(int courseId);
    bool HasGroups(int courseId);
    Task<bool> IsGroupNameUniqueAsync(string name, int? groupId = null);
}
