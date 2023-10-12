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
                .Include(g => g.Students)
                .FirstOrDefault(g => g.Id == id);
            
            return group;
        }
    }
}
