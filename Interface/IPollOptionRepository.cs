using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface
{
    public interface IPollOptionRepository : IRepositoryBase<PollOption>
    {
        Task<List<PollOption>> GetAllByPollId(int id);
    }
}
