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
    public class PollOptionRepository : RepositoryBase<PollOption>, IPollOptionRepository
    {
        public PollOptionRepository(EMSDBContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<PollOption>> GetAllByPollId(int id)
        {
            return await _dbContext.PollOption.Where(x => x.PollId.Equals(id) && x.IsActive.Equals(true)).ToListAsync();
        }
        public async override Task<List<PollOption>> GetAll()
        {
            return await _dbContext.PollOption.OrderBy(x => x.PollId).ThenBy(x=> x.Id).ToListAsync();
        }
    }
}
