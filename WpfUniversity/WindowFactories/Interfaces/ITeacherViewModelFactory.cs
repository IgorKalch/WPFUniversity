using UniversityDataLayer.Entities;
using WpfUniversity.Services;
using WpfUniversity.ViewModels.Teachers;

namespace WpfUniversity.WindowFactories.Interfaces;

public interface ITeacherViewModelFactory
{
    TeacherViewModel Create(WindowService windowService, Teacher teacher = null);
}
