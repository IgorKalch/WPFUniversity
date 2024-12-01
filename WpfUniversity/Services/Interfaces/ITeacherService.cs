using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;

namespace WpfUniversity.Services.Interfaces;

public interface ITeacherService
{
    Task<IEnumerable<Teacher>> GetAllTeachersByCourseIdAsync(int courseId);
    Task<IEnumerable<Teacher>> GetAllTeachersAsync();
    Task<Teacher> GetTeacherByIdAsync(int id);
    Task AddTeacherAsync(Teacher teacher);
    Task UpdateTeacherAsync(Teacher teacher);
    Task DeleteTeacherAsync(int id);
}
