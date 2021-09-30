using Entities.Models;
using Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class PrecinctRepository : RepositoryBase<Precinct>, IPrecinctRepository
    {
        public PrecinctRepository(EMSDBContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }

        public async override Task<Precinct> Get(int id)
        {
            return await _dbContext.Precinct.SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async override Task<List<Precinct>> GetAll()
        {
            return await _dbContext.Precinct.ToListAsync();
        }
    }
}
