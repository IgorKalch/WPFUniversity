using Microsoft.EntityFrameworkCore;
using UniversityDataLayer.Entities;

namespace UniversityDataLayer.Repositories
{
    public class GroupRepositiry : BaseRepository<Group>
    {
        public GroupRepositiry(UniversityContext context) : base(context)
        {
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
}
