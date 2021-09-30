using Entities.Models;
using Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class BarangayRepository : RepositoryBase<Barangay>, IBarangayRepository
    {
        public BarangayRepository(EMSDBContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }

        public async override Task<Barangay> Get(int id)
        {
            return await _dbContext.Barangay.Include(d => d.Cluster).SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async override Task<List<Barangay>> GetAll()
        {
            return await _dbContext.Barangay.Include(d=> d.Cluster).ThenInclude(r=>r.Precinct).ToListAsync();
        }
    }
}
