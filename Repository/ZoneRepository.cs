using Entities.Models;
using Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class ZoneRepository : RepositoryBase<Zone>, IZoneRepository
    {
        public ZoneRepository(EMSDBContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }

        public async override Task<Zone> Get(int id)
        {
            return await _dbContext.Zone.Include(d => d.Barangay).SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async override Task<List<Zone>> GetAll()
        {
            return await _dbContext.Zone.Include(d=> d.Barangay).ThenInclude(r=>r.Cluster).ToListAsync();
        }
    }
}
