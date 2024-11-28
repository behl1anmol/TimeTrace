using System.Linq.Expressions;
using timetrace.library.Context;

namespace timetrace.library.Repositories;
public class RepositoryBase : IRepositoryBase
{
    protected readonly DatabaseContext DbContext;
    protected RepositoryBase(DatabaseContext dbContext)
    {
        this.DbContext = dbContext;
    }
    public TE Add<TE>(TE entity) where TE : class
    {
        DbContext.Set<TE>().Add(entity);
        DbContext.SaveChanges();
        return entity;
    }
    public bool AddAll<TE>(IEnumerable<TE> entities) where TE : class
    {
        DbContext.Set<TE>().AddRange(entities);
        var count = DbContext.SaveChanges();
        return count > 0;
    }

    public void Delete<TE>(TE entity) where TE : class
    {
        try{
            DbContext.Set<TE>().Remove(entity);
            DbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw new Exception($"Error deleting entity of type {typeof(TE).Name}.", e);
        }
    }

    public void DeleteAll<TE>(IEnumerable<TE> entities) where TE : class
    {
        try
        {
            DbContext.Set<TE>().RemoveRange(entities);
            DbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw new Exception($"Error deleting all entities of type {typeof(TE).Name}.", e);
        }
    }

    public void DeleteAll<TE>(Expression<Func<TE, bool>> expression) where TE : class
    {
        try
        {
            var entities = DbContext.Set<TE>().Where(expression);
            DbContext.Set<TE>().RemoveRange(entities);
            DbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw new Exception($"Error deleting all entities of type {typeof(TE).Name}.", e);
        }
    }

    public bool DeleteAll<TE>() where TE : class
    {
        const int batchSize = 1000;
        try
        {
            var dbSet = DbContext.Set<TE>();
            
            while (true)
            {
                var batch = dbSet.Take(batchSize).ToList();
                if (!batch.Any())
                    break;
                    
                dbSet.RemoveRange(batch);
                DbContext.SaveChanges();
            }
            return true;
        }
        catch (Exception e)
        {
            throw new Exception($"Error deleting all entities of type {typeof(TE).Name}.", e);
        }
    }

    public bool Exists<TE>(Expression<Func<TE, bool>> expression) where TE : class
    {
        return DbContext.Set<TE>().Any(expression);
    }

    public bool Exists<TE>(TE entity) where TE : class
    {
        return DbContext.Set<TE>().Contains(entity);
    }

    public TE? Find<TE>(Expression<Func<TE, bool>> expression) where TE : class
    {
        return DbContext.Set<TE>().FirstOrDefault(expression);
    }

    public IEnumerable<TE> FindAll<TE>(Expression<Func<TE, bool>> expression) where TE : class
    {
        return DbContext.Set<TE>().Where(expression).ToList();
    }
    public List<TE> FindAll<TE>(Expression<Func<TE, bool>> expression, int pageSize = 100, int page = 1) where TE : class
    {
        var query = DbContext.Set<TE>().Where(expression).AsQueryable();
        return query.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
    }
    public IEnumerable<TE> FetchAll<TE>() where TE : class
    {
        return DbContext.Set<TE>().ToList();
    }
    public List<TE> FetchAll<TE>(int pageSize = 100, int page = 1) where TE : class
    {
        var query = DbContext.Set<TE>().AsQueryable();
        return query.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
    }

    public TE Update<TE>(TE entity) where TE : class
    {
        DbContext.Set<TE>().Update(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public void UpdateAll<TE>(IEnumerable<TE> entities) where TE : class
    {
        DbContext.Set<TE>().UpdateRange(entities);
        DbContext.SaveChanges();
    }
}
