using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UniversityDataLayer.Entities;

namespace UniversityDataLayer.Repositories;

public class GroupRepositiry : BaseRepository<Group>
{
    public GroupRepositiry(UniversityContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Group>> GetAsync(
            Expression<Func<Group, bool>> filter = null,
            Func<IQueryable<Group>, IOrderedQueryable<Group>> orderBy = null,
            params Expression<Func<Group, object>>[] includeProperties)
    {
        IQueryable<Group> query = _dbSet;

        query = query
            .Include(g => g.Course)
                .ThenInclude(c => c.Groups)
            .Include(g => g.Course)
                .ThenInclude(t => t.Teachers)
            .Include(g => g.Students)
            .Include(t => t.Techer)
                .ThenInclude(c => c.Course)
                    .ThenInclude(g => g.Groups)
            .Include(x => x.Techer)
                .ThenInclude(g => g.Groups);

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

    public override Group GetById(int id)
    {
        var group = _dbSet
            .Include(g => g.Course)
                .ThenInclude(c => c.Groups)
            .Include(g => g.Course)
                .ThenInclude(t => t.Teachers)
            .Include(g => g.Students)
            .Include(t => t.Techer)
                .ThenInclude(c => c.Course)
                    .ThenInclude(g => g.Groups)
            .Include(x => x.Techer)
                .ThenInclude(g => g.Groups)
            .FirstOrDefault(g => g.Id == id);

        return group;
    }
}
