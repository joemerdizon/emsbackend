using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface IActivationTokenRepository : IRepositoryBase<ActivationToken>
    {
        Task<ActivationToken> GetByToken(string token);
        Task<ActivationToken> GetByPersonId(int personId);
    }
}
