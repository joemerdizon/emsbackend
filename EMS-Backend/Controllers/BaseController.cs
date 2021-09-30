using Entities.Models;
using Entities.ViewModel;
using Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EMS_Backend.Controllers
{

    public class BaseController : ControllerBase
    {
        protected IRepositoryWrapper _repoWrapper;
        protected ILogger _logger;
        protected IConfiguration _configuration;

        protected double sessionTimeout = 0;
        protected string GetCurrentUser()
        {
            string currentUser = "";
            if (Request != null && Request.Headers.ContainsKey("Authorization"))
            {
                string bearerToken = Request.Headers["Authorization"];
                string tokenstring = bearerToken.Replace("Bearer ", "").ToString();
                currentUser = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring).Claims.First(c => c.Type == ClaimTypes.Name).Value;
            }
            return currentUser;
        }
        protected int GetCurrentRoleId()
        {
            int roleId = 0;
            if (Request != null && Request.Headers.ContainsKey("Authorization"))
            {
                string bearerToken = Request.Headers["Authorization"];
                string tokenstring = bearerToken.Replace("Bearer ", "").ToString();
                roleId = int.Parse(new JwtSecurityTokenHandler().ReadJwtToken(tokenstring).Claims.First(c => c.Type == ClaimTypes.GroupSid).Value);
            }
            return roleId;
        }
        protected string GetUserFullName(User user)
        {
            string fullName = "";
            if (user != null) {
                fullName = user.Person.FirstName + " " + user.Person.LastName;
            }
            return fullName;
        }

        protected string GetPersonFullName(Person person)
        {
            string fullName = "";
            if (person != null)
            {
                fullName =  person.LastName + ", " + person.FirstName + " " + person.MiddleName;
            }
            return fullName;
        }
        protected string GetVoterFullName(Voter voter)
        {
            string fullName = "";
            if (voter != null)
            {
                fullName = voter.LastName + ", " + voter.FirstName +" "+ voter.MiddleName;
            }
            return fullName;
        }

        protected async Task<List<UserAuthorityViewModel>> GetUserAuthorityRestrictions(int roleId)
        {
            var result = new List<UserAuthorityViewModel>();
            try
            {
                var pmcList = await _repoWrapper.PolicyModuleControl.GetAllByRoleId(roleId);
                foreach (var item in pmcList)
                {
                    result.Add(new UserAuthorityViewModel
                    {
                        moduleId = item.ModuleControl.ModuleId,
                        moduleName = item.ModuleControl.Module.Name,
                        controlId = item.ModuleControl.ControlId,
                        pageId = item.ModuleControl.Module.PageId
                    });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("BaseController: GetUserAuthorityRestrictions" + ex.Message);
            }
            
            return result; ;
        }
        /// <summary>
        /// Combine City, State and  ZIp
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected string CombineCityStateZip (string city, string state, string zip)
        {
            var csz = city + ", " + state + ", " + zip;

            var regex = new Regex("(.*?)(, )?$");

            return regex.Match(csz).Groups[1].Value;
        }
    }
}
