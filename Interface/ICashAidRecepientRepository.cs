using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface
{
    public interface ICashAidRecepientRepository : IRepositoryBase<CashAidRecepients>
    {
        Task<List<CashAidRecepients>> GetAllByEventId(int id);

    }
}
