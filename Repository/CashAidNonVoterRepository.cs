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
    public class CashAidNonVoterRepository : RepositoryBase<CashAidNonVoter>, ICashAidNonVoterRepository
    {
        public CashAidNonVoterRepository(EMSDBContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<CashAidNonVoter>> GetAllByEventId(int id)
        {
            return await _dbContext.CashAidNonVoter.Include(d => d.Event).Include(d => d.Brgy).Where(x => x.EventId.Equals(id) && x.IsActive.Equals(true)).ToListAsync();
        }
        public async override Task<List<CashAidNonVoter>> GetAll()
        {
            return await _dbContext.CashAidNonVoter.Include(d => d.Event).Include(d => d.Brgy).OrderBy(x => x.EventId).ThenBy(x=> x.Id).ToListAsync();
        }
    }
}
