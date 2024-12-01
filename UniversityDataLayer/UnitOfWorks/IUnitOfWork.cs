using UniversityDataLayer.Entities;
using UniversityDataLayer.Repositories;

namespace UniversityDataLayer.UnitOfWorks;

public interface IUnitOfWork : IDisposable
{
    BaseRepository<Course> CourseRepository { get; }
    BaseRepository<Group> GroupRepository { get; }
    BaseRepository<Student> StudentRepository { get; }
    BaseRepository<Teacher> TeacherRepository { get; }

    void Commit();
    Task CommitAsync();
}
