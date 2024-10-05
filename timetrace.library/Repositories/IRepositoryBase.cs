using System.Linq.Expressions;

namespace timetrace.library.Repositories;

public interface IRepositoryBase
{
    TE Add<TE>(TE entity) where TE : class;
    TE Update<TE>(TE entity) where TE : class;
    void Delete<TE>(TE entity) where TE : class;
    void DeleteAll<TE>(IEnumerable<TE> entities) where TE : class;
    void DeleteAll<TE>(Expression<Func<TE, bool>> expression) where TE : class;
    bool Exists<TE>(Expression<Func<TE, bool>> expression) where TE : class;
    bool Exists<TE>(TE entity) where TE : class;
    TE? Find<TE>(Expression<Func<TE,bool>> expression) where TE : class;
    IEnumerable<TE> FindAll<TE>(Expression<Func<TE, bool>> expression) where TE : class;
    IEnumerable<TE> FetchAll<TE>() where TE : class;
}
