using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using timetrace.library.Context;

namespace timetrace.library.Repositories;
public class ProcessRepository(DatabaseContext dbContext) : RepositoryBase(dbContext), IProcessRepository
{
}
