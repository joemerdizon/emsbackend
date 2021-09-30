using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        /// <summary>
        /// Get All Available location that has the same ItemId and warehouse ID regardless if zero or more Qty
        /// </summary>
        /// <param name="name">name from usert table Id</param>
        /// <returns></returns>
        Task<User> GetByUserName(string name);

        Task<User> GetByPersonId(int id);
    }
}
