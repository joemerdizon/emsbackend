using Entities.Models;
using Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(EMSDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async override Task<List<User>> GetAll()
        {
            return await _dbContext.User.Include(d => d.Person).ToListAsync();
        }
        public async Task<User> GetByUserName(string name)
        {
            return await _dbContext.User.Include(d => d.UserRole).Include(d => d.Person)
                                        .SingleOrDefaultAsync(u => u.UserName.Equals(name));
        }

        public async override Task<User> Get(int id)
        {
            return await _dbContext.User.Include(d => d.UserRole).Include(d => d.Person).SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async Task<User> GetByPersonId(int id)
        {
            return await _dbContext.User.Include(d => d.UserRole).Include(d => d.Person).SingleOrDefaultAsync(x => x.PersonId.Equals(id));
        }

    }
}
