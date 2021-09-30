using Entities.Models;
using Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class ClusterRepository : RepositoryBase<Cluster>, IClusterRepository
    {
        public ClusterRepository(EMSDBContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }

        public async override Task<Cluster> Get(int id)
        {
            return await _dbContext.Cluster.Include(d => d.Precinct).SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async override Task<List<Cluster>> GetAll()
        {
            return await _dbContext.Cluster.Include(d=> d.Precinct).ToListAsync();
        }
    }
}
