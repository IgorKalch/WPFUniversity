using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;

namespace WpfUniversity.Services.Interfaces;

public interface ITeacherService
{
    List<Teacher> Teachers { get; }
    Task<IEnumerable<Teacher>> GetAllTeachersByCourseIdAsync(int courseId);
    Task Load();
    Task<Teacher> GetTeacherByIdAsync(int id);
    Task AddTeacherAsync(Teacher teacher);
    Task UpdateTeacherAsync(Teacher teacher);
    Task DeleteTeacherAsync(Teacher teacher);
}
