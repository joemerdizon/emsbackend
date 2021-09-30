using Entities.Models;
using Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class DistrictRepository : RepositoryBase<District>, IDistrictRepository
    {
        public DistrictRepository(EMSDBContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }

        public async override Task<District> Get(int id)
        {
            return await _dbContext.District.Include(d => d.Zone).SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async override Task<List<District>> GetAll()
        {
            return await _dbContext.District.Include(d=> d.Zone).ThenInclude(r=>r.Barangay).ToListAsync();
        }
    }
}
