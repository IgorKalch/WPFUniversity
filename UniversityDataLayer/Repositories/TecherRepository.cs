using Microsoft.EntityFrameworkCore;
using UniversityDataLayer.Entities;

namespace UniversityDataLayer.Repositories;

public class TeacherRepository : BaseRepository<Teacher>
{
    public TeacherRepository(UniversityContext context) : base(context)
    {                
    }

    public override Teacher GetById(int id)
    {
        var teacher = _dbSet
             .Include(c => c.Course)
                .ThenInclude(g => g.Groups)
            .Include(g => g.Groups)
                .ThenInclude(s => s.Students)
            .FirstOrDefault(x => x.Id == id);

        return teacher;
    }
}
