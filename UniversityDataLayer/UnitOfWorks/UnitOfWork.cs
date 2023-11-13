using Microsoft.EntityFrameworkCore;
using UniversityDataLayer.Entities;
using UniversityDataLayer.Repositories;

namespace UniversityDataLayer.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UniversityContext _dbContext;
        private bool _disposed = false;

        private BaseRepository<Group>? _groupRepository;
        private BaseRepository<Course>? _courseRepository;
        private BaseRepository<Student>? _studentRepository;
        private BaseRepository<Teacher>? _teacherRepository;

        public BaseRepository<Student> StudentRepository => _studentRepository ?? (this._studentRepository = new StudentRepositiry(_dbContext));
        public BaseRepository<Group> GroupRepository => _groupRepository ?? (this._groupRepository = new GroupRepositiry(_dbContext));
        public BaseRepository<Course> CourseRepository => _courseRepository ?? (this._courseRepository = new CourseRepository(_dbContext));
        public BaseRepository<Teacher> TeacherRepository => _teacherRepository ?? (this._teacherRepository = new TeacherRepository(_dbContext));

        public UnitOfWork(UniversityContext dbContext)
        {
            _dbContext = dbContext;
        }     

        public void Commit()
        {
            _dbContext.SaveChanges();
        }    

        public virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
                this._disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
