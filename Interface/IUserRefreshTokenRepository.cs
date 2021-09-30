using Entities.Models;
using System.Threading.Tasks;

namespace Interface
{
    public interface IUserRefreshTokenRepository
    {
        Task<bool> SaveOrUpdateUserRefreshToken(UserRefreshToken userRefreshToken);

        Task<bool> CheckIfRefreshTokenIsValid(string username, string refreshToken);
    }
}
