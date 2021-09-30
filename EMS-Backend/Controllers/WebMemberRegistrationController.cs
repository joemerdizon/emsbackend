using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS_Backend.Helper;
using Entities.Enums;
using Entities.Models;
using Entities.ViewModel;
using Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BC = BCrypt.Net.BCrypt;
using EMS_Backend.SMTP;
using EMS_Backend.SMS;


namespace EMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebMemberRegistrationController : BaseController
    {
        public WebMemberRegistrationController(IRepositoryWrapper repoWrapper, ILogger<WebMemberRegistrationController> logger, IConfiguration configuration)
        {
            _repoWrapper = repoWrapper;
            _logger = logger;
            _configuration = configuration;
            TimeSpan expiryHours = TimeSpan.FromHours(int.Parse(_configuration["JWT:ExpiryHours"]));
            sessionTimeout = expiryHours.TotalMinutes;
        }

        //[HttpGet]
        //[Route("Activate/{id}")]
        //public async Task<ActionResult<ApiResponse>> Activate(Guid id)
        //{
        //    try
        //    {
        //        var result = _repoWrapper.Person.GetPersonByToken(id);

        //        var model = new PersonActivationRequest();

        //        if (result != null && Convert.ToBoolean(result.Result.message) == false && result.Result.personId == 0)
        //        {
        //            model.personId = result.Result.personId;

        //            model.message = "Invalid Activation Code";
        //            model.activationCode = id.ToString();
        //        }
        //        else if (result != null && Convert.ToBoolean(result.Result.message) == false && result.Result.personId != 0)
        //        {
        //            model.personId = result.Result.personId;
        //            model.message = "Activation Code Expired";
        //            model.activationCode = id.ToString();
        //        }
        //        else if(result != null && Convert.ToBoolean(result.Result.message) == true && result.Result.personId != 0)
        //        {
        //            model.personId = result.Result.personId;
        //            model.message = "Valid Activation";
        //            model.activationCode = id.ToString();
        //        }

        //        return Ok(new ApiResponse { data = model, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Person: Edit" + ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
        //    }
        //}

        [HttpGet]
        [Route("Activate/{id}")]
        public async Task<ActionResult<ApiResponse>> Activate(string id)
        {
            try
            {
                var result = _repoWrapper.Person.GetPersonByToken(id);

                var model = new PersonActivationRequest();

                if (result != null && Convert.ToBoolean(result.Result.message) == false && result.Result.personId == 0)
                {
                    model.personId = result.Result.personId;
                    model.message = "Invalid Activation Code";
                    model.activationCode = id.ToString();
                }
                else if (result != null && Convert.ToBoolean(result.Result.message) == false && result.Result.personId != 0)
                {
                    model.personId = result.Result.personId;
                    model.message = "Activation Code Expired";
                    model.activationCode = id.ToString();
                }
                else if (result != null && Convert.ToBoolean(result.Result.message) == true && result.Result.personId != 0)
                {
                    model.personId = result.Result.personId;
                    model.message = "Valid Activation";
                    model.activationCode = id.ToString();
                }

                return Ok(new ApiResponse { data = model, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Person: Edit" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        [HttpPut]
        [Route("MemberActivate/{id}")]
        public async Task<ActionResult<ApiResponse>> MemberActivate(int id, [FromBody] PersonActivationRequest model)
        {
            try
            {
                var personModel = await _repoWrapper.Person.Get(model.personId);
                personModel.IsMember = true;
                await _repoWrapper.Person.Update(personModel);

                var personUserAcc = await _repoWrapper.User.GetByPersonId(model.personId);
                string PasswordHash = BC.HashPassword(model.password);
                personUserAcc.UserName = model.username;
                personUserAcc.Password = PasswordHash;
                personUserAcc.Email = personModel.EmailAddress;
                await _repoWrapper.User.Update(personUserAcc);

                //var actModel = await _repoWrapper.ActivationToken.GetByToken(model.activationCode);
                var actModel = await _repoWrapper.ActivationToken.GetByPersonId(model.personId);
                actModel.IsActive = false;
                await _repoWrapper.ActivationToken.Update(actModel);

                return Ok(new ApiResponse { data = PageResult.Success, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Person: Edit" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = PageResult.Failed, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        [HttpPost]
        [Route("GetByUserName/{username}")]
        public bool ValidateUsername(string username)
        {
            var userModel = _repoWrapper.User.GetByUserName(username).GetAwaiter().GetResult();
            if (userModel != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
