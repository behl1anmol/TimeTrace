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
        DbContext.Set<TE>().Remove(entity);
        DbContext.SaveChanges();
    }

    public void DeleteAll<TE>(IEnumerable<TE> entities) where TE : class
    {
        DbContext.Set<TE>().RemoveRange(entities);
        DbContext.SaveChanges();
    }

    public void DeleteAll<TE>(Expression<Func<TE, bool>> expression) where TE : class
    {
        var entities = DbContext.Set<TE>().Where(expression);
        DbContext.Set<TE>().RemoveRange(entities);
        DbContext.SaveChanges();
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
    public IEnumerable<TE> FindAll<TE>(Expression<Func<TE, bool>> expression, int pageSize, int page) where TE : class
    {
        return DbContext.Set<TE>().Where(expression).Skip(page * pageSize).Take(pageSize);
    }
    public IEnumerable<TE> FetchAll<TE>() where TE : class
    {
        return DbContext.Set<TE>().ToList();
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
