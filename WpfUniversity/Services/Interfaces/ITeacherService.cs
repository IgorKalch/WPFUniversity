using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;

namespace WpfUniversity.Services.Interfaces;

public interface ITeacherService
{
    Task<IEnumerable<Teacher>> GetAllTeachersByCourseIdAsync(int courseId);
}
