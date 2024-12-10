using UniversityDataLayer.Entities;
using WpfUniversity.Services.Interfaces;
using WpfUniversity.Views.Students;

namespace WpfUniversity.WindowFactories.Interfaces;

public interface IStudentsWindowFactory
{
    StudentsWindow Create(Group selectedGroup, IWindowService windowService);
}
