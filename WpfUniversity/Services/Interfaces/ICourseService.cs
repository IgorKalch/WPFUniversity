using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;

namespace WpfUniversity.Services.Interfaces;

public interface ICourseService
{
    List<Course> Courses { get; }
    Task Load();
    Task Add(Course course);
    Task Update(Course course);
    Task Delete(Course course);
}