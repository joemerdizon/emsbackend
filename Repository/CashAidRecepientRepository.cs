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
    public class CashAidRecepientRepository : RepositoryBase<CashAidRecepients>, ICashAidRecepientRepository
    {
        public CashAidRecepientRepository(EMSDBContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<CashAidRecepients>> GetAllByEventId(int id)
        {
            return await _dbContext.CashAidRecepients.Include(d => d.Event).Include(d => d.Person)
                .ThenInclude(x => x.Brgy).ThenInclude(x => x.Zone).ThenInclude(m => m.District)
                .Include(d => d.Person).ThenInclude(x => x.Cluster).ThenInclude(x => x.Precinct)
                .Where(x => x.EventId.Equals(id) && x.IsActive.Equals(true)).ToListAsync();
        }
        public async override Task<List<CashAidRecepients>> GetAll()
        {
            return await _dbContext.CashAidRecepients.Include(d => d.Event).Include(d => d.Person).OrderBy(x => x.EventId).ThenBy(x=> x.Id).ToListAsync();
        }
    }
}
