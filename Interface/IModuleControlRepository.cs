using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface
{
    public interface IModuleControlRepository : IRepositoryBase<ModuleControl>
    {
        Task<List<ModuleControl>> GetAllByModuleId(int id);


    }
}
