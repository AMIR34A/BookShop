using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookShop.Models.Repository;

public class RepositoryBase<TEntity, TContext> : IRepositoryBase<TEntity>,IDisposable where TContext : DbContext where TEntity : class
{
    protected TContext _context;
    private DbSet<TEntity> _dbSet;
    private bool disposedValue;

    public RepositoryBase(TContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }
    public async Task CreateAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task CreateRangeAsync(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, params Expression<Func<TEntity, bool>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;
        foreach (var include in includes)
            query = _dbSet.Include(include);

        if (filter is not null)
            query = _dbSet.Where(filter);

        if (orderBy is not null)
            query = orderBy(query);

        return await query.ToListAsync();
    }

    public async Task<TEntity> FindByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public void Update(TEntity entity) => _dbSet.Update(entity);

    public void UpdateRange(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
