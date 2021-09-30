using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface
{
    public interface IPolicyModuleControlRepository : IRepositoryBase<PolicyModuleControl>
    {
        Task<List<PolicyModuleControl>> GetAllByRoleId(int roleId);
    }
}
