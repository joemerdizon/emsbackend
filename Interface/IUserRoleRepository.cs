using Entities.Models;
using System.Threading.Tasks;

namespace Interface
{
    public interface IUserRoleRepository : IRepositoryBase<UserRole>
    {
        Task<UserRole> GetByName(string Name);
    }
}
