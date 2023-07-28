using System.Linq.Expressions;

namespace BookShop.Models.Repository;

public interface IRepositoryBase<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> FindByIdAsync(object id);
    Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, params Expression<Func<TEntity, bool>>[] includes);
    Task CreateAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task CreateRangeAsync(IEnumerable<TEntity> entities);
    void UpdateRange(IEnumerable<TEntity> entities);
    void DeleteRange(IEnumerable<TEntity> entities);
    Task<List<TEntity>> GetPaginateResaultAsync(int currentPage, int pageSize);
    Task<int> CountAsync();
    void Dispose();
}
