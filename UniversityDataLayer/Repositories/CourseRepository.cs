using Microsoft.EntityFrameworkCore;
using UniversityDataLayer.Entities;

namespace UniversityDataLayer.Repositories;

public class CourseRepository : BaseRepository<Course>
{
    public CourseRepository(UniversityContext context) : base(context)   
    {
    }

    public override Course GetById(int id)
    {
        var course = _dbSet
             .Include(c => c.Groups)
                 .ThenInclude(g => g.Students)
              .Include(t => t.Teachers)
                 .ThenInclude(g => g.Groups)
             .FirstOrDefault(x => x.Id == id);

        return course;
    }
}
