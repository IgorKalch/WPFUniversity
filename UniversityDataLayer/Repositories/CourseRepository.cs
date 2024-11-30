using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UniversityDataLayer.Entities;

namespace UniversityDataLayer.Repositories;

public class CourseRepository : BaseRepository<Course>
{
    public CourseRepository(UniversityContext context) : base(context)
    {
    }
    public override async Task<IEnumerable<Course>> GetAsync(
            Expression<Func<Course, bool>> filter = null,
            Func<IQueryable<Course>, IOrderedQueryable<Course>> orderBy = null,
            params Expression<Func<Course, object>>[] includeProperties)
    {
        IQueryable<Course> query = _dbSet;

        query = query
            .Include(c => c.Groups)
                .ThenInclude(g => g.Students)
            .Include(c => c.Teachers)
                .ThenInclude(t => t.Groups);

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includeProperties != null && includeProperties.Any())
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return (orderBy != null) ? await orderBy(query).ToListAsync() : await query.ToListAsync();
    }

    public override Course? GetById(int id)
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
