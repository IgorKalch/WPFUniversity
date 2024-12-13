using UniversityDataLayer.Entities;
using WpfUniversity.Services.Interfaces;
using WpfUniversity.ViewModels.Students;

namespace WpfUniversity.WindowFactories.Interfaces;

public interface IStudentsViewModelFactory
{
    StudentsViewModel Create(Group selectedGroup, IWindowService windowService);
}
