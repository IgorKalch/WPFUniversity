using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;

namespace WpfUniversity.Services.Interfaces;

public interface IStudentService
{
    List<Student> Students { get; }
    Task LoadStudentsByGroup(int groupId);
    Task Add(Student student);
    Task Update(Student student);
    Task Delete(Student student);
}
