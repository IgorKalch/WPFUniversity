using UniversityDataLayer.Entities;
using WpfUniversity.Views.Groups;

namespace WpfUniversity.WindowFactories.Interfaces;

public interface IGroupsWindowFactory
{
    GroupsWindow Create(Course selectedCourse);
}