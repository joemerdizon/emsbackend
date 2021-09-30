using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface
{
    public interface ICashAidNonVoterRepository : IRepositoryBase<CashAidNonVoter>
    {
        Task<List<CashAidNonVoter>> GetAllByEventId(int id);
    }
}
