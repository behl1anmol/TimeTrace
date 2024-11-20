using timetrace.library.Context;

namespace timetrace.library.Repositories;
public class ProcessRepository(DatabaseContext dbContext) : RepositoryBase(dbContext), IProcessRepository
{
}
