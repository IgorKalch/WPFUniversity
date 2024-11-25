using UniversityDataLayer.Entities;
using WpfUniversity.ViewModels.Groups;

namespace WpfUniversity.WindowFactories.Interfaces;

public interface IGroupsViewModelFactory
{
    GroupsViewModel Create(Course selectedCourse);
}
