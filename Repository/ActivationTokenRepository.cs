using Entities.Models;
using Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ActivationTokenRepository : RepositoryBase<ActivationToken>, IActivationTokenRepository
    {
        public ActivationTokenRepository(EMSDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async override Task<ActivationToken> Get(int id)
        {
            return await _dbContext.ActivationToken.Include(d => d.Person).SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async override Task<List<ActivationToken>> GetAll()
        {
            return await _dbContext.ActivationToken.Include(d => d.Person).ToListAsync();
        }

        public async Task<ActivationToken> GetByToken(string token)
        {
            return await _dbContext.ActivationToken.Include(d => d.Person).SingleOrDefaultAsync(x => x.Token.Equals(token) && x.IsActive == true);
        }

        public async Task<ActivationToken> GetByPersonId(int personId)
        {
            return await _dbContext.ActivationToken.Include(d => d.Person).SingleOrDefaultAsync(x => x.PersonId.Equals(personId) && x.IsActive == true);
        }

    }
}
