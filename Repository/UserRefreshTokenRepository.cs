using Entities.Models;
using Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRefreshTokenRepository : IUserRefreshTokenRepository
    {

        private EMSDBContext _dbContext;
        /// <summary>
        /// This class is for implementation of JWT
        /// </summary>
        /// <param name="dbContext"></param>
        public UserRefreshTokenRepository(EMSDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CheckIfRefreshTokenIsValid(string username, string refreshToken)
        {
            var token = await _dbContext.UserRefreshToken.SingleOrDefaultAsync(u => u.UserName.Equals(username));

            if (token != null)
            {
                return  token.RefreshToken.Equals(refreshToken);
            }

            return false;
        }

        public async Task<bool> SaveOrUpdateUserRefreshToken(UserRefreshToken userRefreshToken)
        {
            var token = await _dbContext.UserRefreshToken.SingleOrDefaultAsync(u => u.UserName.Equals(userRefreshToken.UserName));
            try
            {
                if (token != null)
                {
                    token.RefreshToken = userRefreshToken.RefreshToken;
                    token.Token = userRefreshToken.Token;
                    _dbContext.Entry(token).State = EntityState.Modified;
                }
                else
                {
                    _dbContext.Set<UserRefreshToken>().Add(userRefreshToken);
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
