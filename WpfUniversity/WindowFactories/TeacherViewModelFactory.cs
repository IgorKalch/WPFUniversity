using UniversityDataLayer.Entities;
using WpfUniversity.Services;
using WpfUniversity.Services.Interfaces;
using WpfUniversity.ViewModels.Teachers;
using WpfUniversity.WindowFactories.Interfaces;

namespace WpfUniversity.WindowFactories;

public class TeacherViewModelFactory : ITeacherViewModelFactory
{
    private readonly ITeacherService _teacherService;
    private readonly ICourseService _courseService;

    public TeacherViewModelFactory(ITeacherService teacherService, ICourseService courseService)
    {
        _teacherService = teacherService;
        _courseService = courseService;
    }

    public TeacherViewModel Create(WindowService windowService, Teacher teacher = null)
    {
        return new TeacherViewModel(windowService, _teacherService, _courseService, teacher);
    }
}
