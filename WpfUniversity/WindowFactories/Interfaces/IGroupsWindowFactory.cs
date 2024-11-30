using UniversityDataLayer.Entities;
using WpfUniversity.Services;
using WpfUniversity.Views.Groups;

namespace WpfUniversity.WindowFactories.Interfaces;

public interface IGroupsWindowFactory
{
    GroupsWindow Create(Course selectedCourse, WindowService windowService);
}