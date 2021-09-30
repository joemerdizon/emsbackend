using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using EMS_Backend.Helper;
using Entities.Models;
using Entities.ViewModel;
using Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;

namespace EMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        public AuthenticationController(IRepositoryWrapper repoWrapper, IConfiguration configuration)
        {
            _repoWrapper = repoWrapper;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginViewModel model)
        {
            try
            {
                
                var user = await _repoWrapper.User.GetByUserName(model.UserName);
                if (user != null && BC.Verify(model.Password, user.Password))
                {
                    var jwtToken = GenerateJwtToken(user);
                    var result = new UserRefreshTokenViewModel
                    {
                        id = user.Id,
                        firstName = user.Person.FirstName,
                        lastName = user.Person.LastName,
                        token = jwtToken.Token,
                        refreshToken = jwtToken.RefreshToken
                    };

                    await _repoWrapper.UserRefreshToken.SaveOrUpdateUserRefreshToken(new UserRefreshToken { UserName = user.UserName, Token = jwtToken.Token, RefreshToken = jwtToken.RefreshToken });
                    var restrictions = await GetUserAuthorityRestrictions(user.UserRoleId);

                    return Ok(new ApiResponse { data = result, supportingLists = new {  }, sessionTimeout = jwtToken.SessionTimeout, restrictions = restrictions });
                }
                else
                {
                    return Unauthorized(new ApiResponse { errors = "Error", message = "Invalid UserName/Password" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { errors = ex.Message, message = ex.InnerException.Message });
            }
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<ActionResult<ApiResponse>> RefreshToken([FromBody] JwtToken jwtToken)
        {
            if (jwtToken == null)
            { 
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { errors = "Error", message = "Invalid Request" });
            }
            try
            {
                SecurityToken validatedToken;
                var handler = new JwtSecurityTokenHandler();
                IPrincipal principal = handler.ValidateToken(jwtToken.Token, GetTokenValidationParameters(), out validatedToken);
                var timeout = principal.Identity;
                var userName = principal.Identity.Name;

                if (await _repoWrapper.UserRefreshToken.CheckIfRefreshTokenIsValid(userName, jwtToken.RefreshToken))
                {
                    var user = await _repoWrapper.User.GetByUserName(userName);

                    var newJwtToken = GenerateJwtToken(user);

                    await _repoWrapper.UserRefreshToken.SaveOrUpdateUserRefreshToken(new UserRefreshToken
                    {
                        UserName = user.UserName,
                        Token = newJwtToken.Token,
                        RefreshToken = newJwtToken.RefreshToken
                    });

                    var result = new UserRefreshTokenViewModel
                    {
                        id = user.Id,
                        firstName = user.Person.FirstName,
                        lastName = user.Person.LastName,
                        token = newJwtToken.Token,
                        refreshToken = newJwtToken.RefreshToken
                    };

                    return Ok(new ApiResponse { data = result, sessionTimeout = newJwtToken.SessionTimeout });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { errors = ex.Message, message = ex.InnerException.Message });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { errors = "Error", message = "Invalid request please contact system Administrator" });
        }
        private TokenValidationParameters GetTokenValidationParameters()
        { 
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidAudience = _configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]))
            };
        }
        private JwtToken GenerateJwtToken(User model) 
        {
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.UserName),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            authClaims.Add(new Claim(ClaimTypes.Role, model.UserRole.Role));
            authClaims.Add(new Claim(ClaimTypes.GroupSid, model.UserRoleId.ToString()));

            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(int.Parse(_configuration["JWT:ExpiryHours"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
                );

            TimeSpan ts = token.ValidTo.Subtract(DateTime.UtcNow);

            var jwtToken = new JwtToken
            {
                RefreshToken = new RefreshTokenGenerator().GenerateRefreshToken(32),
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                SessionTimeout = ts.TotalMinutes
            };

            return  jwtToken;
        }

        #region Helper Methods
       
        #endregion
    }
}
