using System.Linq.Expressions;
using UniversityDataLayer.Entities;

namespace UniversityDataLayer.Repositories;

public interface IRepository<T> where T : Entity
{
    IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includeProperties);
    T GetById(int id);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
}
