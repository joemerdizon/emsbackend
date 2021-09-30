using EMS_Backend.Helper;
using Entities.Enums;
using Entities.Models;
using Entities.ViewModel;
using Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS_Backend.Controllers
{
    [RoleAuthorize("superadmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PollController : BaseController
    {
        public PollController(IRepositoryWrapper repoWrapper, ILogger<PollController> logger, IConfiguration configuration)
        {
            _repoWrapper = repoWrapper;
            _logger = logger;
            _configuration = configuration;
            TimeSpan expiryHours = TimeSpan.FromHours(int.Parse(_configuration["JWT:ExpiryHours"]));
            sessionTimeout = expiryHours.TotalMinutes;
        }

        // POST: api/<PollController>/list
        [HttpPost]
        [Route("List")]
        public async Task<ActionResult<ApiResponse>> List(bool includeDeleted, [FromBody] ListSettings ls)
        {
            try
            {
                var polls = await _repoWrapper.Poll.EfficientGetAll(ls);
                var userList = await _repoWrapper.User.GetAll();                

                var result = (from poll in polls.Item1
                              join user in userList on poll.CreatedBy equals user.Id into u
                              from uu in u.DefaultIfEmpty()

                              select new PollViewModel
                              {
                                  id = poll.Id,
                                  pollDescription = poll.PollDescription,                                  
                                  expiryDate = poll.ExpiryDate,
                                  isActive = (bool)poll.IsActive,
                                  status = poll.ExpiryDate != null ? poll.ExpiryDate.Value.Date >= DateTime.Now.Date ? "Active" : "Expired" : "Active",
                                  updatedBy = (uu == null ? String.Empty : GetUserFullName(uu)),
                                  updatedDate = poll.UpdatedDate
                              }).ToList();

                result = includeDeleted ? result : result.Where(x => x.status == "Active").ToList();

                return Ok(new ApiResponse { data = result, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("PollManagement: List" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<ActionResult<ApiResponse>> Edit(int id)
        {
            try
            {
                var model = await _repoWrapper.Poll.Get(id);

                var result = new PollUpdateViewModel();
                if (model != null)
                {
                    result = new PollUpdateViewModel
                    {
                        id = model.Id,
                        pollDescription = model.PollDescription,
                        expiryDate = model.ExpiryDate,
                        updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : ""
                    };
                }
                return Ok(new ApiResponse { data = result, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Poll: Edit" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }
              
        [HttpPut]
        [Route("Save/{id}")]
        public async Task<ActionResult<ApiResponse>> Save(int id, [FromBody] PollUpdateViewModel model)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

                return StatusCode(StatusCodes.Status500InternalServerError,
                                new ApiResponse { data = null, errors = errors, message = "Error exception occured", sessionTimeout = sessionTimeout });
            }
            try
            {
                if (id != model.id)
                {
                    return BadRequest(new ApiResponse { errors = "Bad Request", message = "Bad Request", sessionTimeout = sessionTimeout });
                }

                if (id > 0)
                {
                    var poll = await _repoWrapper.Poll.Get(id);
                    poll.PollDescription = model.pollDescription;
                    poll.ExpiryDate = model.expiryDate;
                    poll.IsActive = true;
                    poll.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                    poll.UpdatedDate = DateTime.Now;
                    await _repoWrapper.Poll.Update(poll);

                    if (model.pollOptions != null && model.pollOptions.Count > 0)
                    {
                        foreach (var option in model.pollOptions.Where(x => x.id == 0))
                        {
                            var opt = new PollOption
                            {
                                Poll = poll,
                                PollId = poll.Id,
                                OptionDescription = option.optionDescription,
                                IsActive = true
                            };

                            opt = await _repoWrapper.PollOption.Add(opt);
                        }
                    }

                }
                else
                {
                    var poll = new Poll
                    {
                        PollDescription = model.pollDescription,
                        ExpiryDate = model.expiryDate,
                        IsActive = true,
                        UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null,
                        UpdatedDate = DateTime.Now
                    };
                    poll = await _repoWrapper.Poll.Add(poll);
                    model.id = poll.Id;

                    if (model.pollOptions != null && model.pollOptions.Count > 0)
                    {
                        foreach (var option in model.pollOptions.Where(x => x.id == 0))
                        {
                            var opt = new PollOption
                            {
                                Poll = poll,
                                PollId = poll.Id,
                                OptionDescription = option.optionDescription,
                                IsActive = true
                            };

                            opt = await _repoWrapper.PollOption.Add(opt);
                        }
                    }
                }
                return Ok(new ApiResponse { data = model, message = "Successfully " + (id <= 0 ? "Created" : "Updated"), sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Poll: Save" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        [HttpPost]
        [Route("ListOptions")]
        public async Task<ActionResult<ApiResponse>> ListOptions(int id)
        {
            try
            {
                var options = await _repoWrapper.PollOption.GetAllByPollId(id);

                var result = (from p in options

                              select new PollOptionsViewModel
                              {
                                  id = p.Id,
                                  pollId = p.PollId,
                                  optionDescription = p.OptionDescription,
                                  isActive = (bool)p.IsActive,
                                  updatedBy = "",
                                  updatedDate = p.UpdatedDate
                              }).ToList();

                return Ok(new ApiResponse { data = result, dataCount = result.Count, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Event: ListOptions" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }
    }
}
