using Microsoft.EntityFrameworkCore;
using UniversityDataLayer.Entities;

namespace UniversityDataLayer.Repositories;

public class StudentRepositiry : BaseRepository<Student>
{
    public StudentRepositiry(UniversityContext context) : base(context)
    {
    }

    public override Student GetById(int id)
    {
        var student = _dbSet
            .Include(s => s.Group)
                .ThenInclude(g => g.Course)
            .Include(s => s.Group)
                .ThenInclude(g => g.Students)
            .FirstOrDefault(x => x.Id == id);

        return student;
    }
}
