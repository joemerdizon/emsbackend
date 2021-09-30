using Entities.Models;
using Interface;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRoleRepository : RepositoryBase<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(EMSDBContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<UserRole> GetByName(string Name)
        {
            return await _dbContext.UserRole.SingleOrDefaultAsync(u => u.Role.Equals(Name));
        }
    }
}
