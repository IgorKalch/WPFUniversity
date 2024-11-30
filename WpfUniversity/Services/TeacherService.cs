using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.Services.Interfaces;

namespace WpfUniversity.Services;

public class TeacherService : ITeacherService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly List<Teacher> _teachers = [];
    public List<Teacher> Teachers => _teachers;

    public TeacherService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Teacher>> GetAllTeachersByCourseIdAsync(int courseId)
    {
        List<Teacher> result = [];

        var studentsToAdd = await _unitOfWork.TeacherRepository.GetAsync(x => x.CourseId == courseId);

        foreach (var student in studentsToAdd)
        {
            var studentToAdd = _unitOfWork.TeacherRepository.GetById(student.Id);
            result.Add(studentToAdd);
        }

        return result;
    }
}
