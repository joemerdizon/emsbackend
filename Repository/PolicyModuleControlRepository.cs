using Entities.Models;
using Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PolicyModuleControlRepository : RepositoryBase<PolicyModuleControl>, IPolicyModuleControlRepository
    {
        public PolicyModuleControlRepository(EMSDBContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PolicyModuleControl>> GetAllByRoleId(int roleId)
        {
            return await _dbContext.PolicyModuleControl.Include(x => x.Policy).ThenInclude(x => x.PolicyRoles).ThenInclude(x => x.Role)
                                                       .Include(x => x.ModuleControl).ThenInclude(x => x.Module)
                                                       .Where(x => x.IsActive.Equals(true) && x.Policy.IsActive.Equals(true) && x.IsRestricted
                                                       && x.Policy.PolicyRoles.Where(m => m.RoleId == roleId).Any()).ToListAsync();
        }
    }
}
