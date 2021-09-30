using Entities.Models;
using Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class PolicyRepository : RepositoryBase<Policy>, IPolicyRepository
    {
        public PolicyRepository(EMSDBContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }

        public async override Task<Policy> Get(int id)
        {
            return await _dbContext.Policy.Include(d => d.PolicyRoles)
                                          .Include(d => d.PolicyModuleControl).SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async override Task<List<Policy>> GetAll()
        {
            return await _dbContext.Policy.Include(d=> d.PolicyRoles).ThenInclude(r=>r.Role)
                                          .Include(d => d.PolicyModuleControl).ToListAsync();
        }
    }
}
