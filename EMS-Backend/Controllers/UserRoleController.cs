using System;
using System.Linq;
using System.Threading.Tasks;
using EMS_Backend.Helper;
using Entities.Models;
using Entities.ViewModel;
using Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EMS_Backend.Controllers
{
    [RoleAuthorize("superadmin, admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : BaseController
    {
        public UserRoleController(IRepositoryWrapper repositoryWrapper, ILogger<UserRoleController> logger, IConfiguration configuration)
        {
            _repoWrapper = repositoryWrapper;
            _logger = logger;
            _configuration = configuration;
            TimeSpan expiryHours = TimeSpan.FromHours(int.Parse(_configuration["JWT:ExpiryHours"]));
            sessionTimeout = expiryHours.TotalMinutes;
        }
        // GET: api/<UserRoleController>/list
        [HttpGet]
        [Route("List/{includeDeleted}")]
        public async Task<ActionResult<ApiResponse>> List(bool includeDeleted)
        {
            try
            {
                var userRoles = await _repoWrapper.UserRole.GetAll();
                var userList = await _repoWrapper.User.GetAll();

                userRoles = includeDeleted ? userRoles : userRoles.Where(x => x.IsActive == true).ToList();
                var result = (from role in userRoles
                              join user in userList on role.UpdatedBy equals user.Id into u
                              from uu in u.DefaultIfEmpty()

                              select new RoleViewModel
                              {
                                  id = role.Id,
                                  role = role.Role,
                                  isActive = (bool)role.IsActive,
                                  updatedBy = (uu == null ? String.Empty : GetUserFullName(uu)),
                                  updatedDate = role.UpdatedDate
                              }).ToList();
                return Ok(new ApiResponse { data = result, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId())});
            }
            catch (Exception ex)
            {
                _logger.LogError("UserRole: List" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }
        
        // GET api/<UserRoleController>/detail/5
        [HttpGet]
        [Route("Detail/{id}")]
        public async Task<ActionResult<ApiResponse>> Detail(int id, [FromBody] string message)
        {
            try
            {
                var model = await _repoWrapper.UserRole.Get(id);
                if (model == null)
                {
                    return NotFound(new ApiResponse { errors = "Item Not Found", message = "Could not found Item", sessionTimeout = sessionTimeout });
                }

                var result = new RoleViewModel
                {
                    id = model.Id,
                    role = model.Role,
                    updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : "",
                    updatedDate = model.UpdatedDate
                };
                return Ok(new ApiResponse { data = result, message = message != null ? message : "", sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Useer Role: Edit" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // GET api/<UserRoleController>/edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<ActionResult<ApiResponse>> Edit(int id)
        {
            try
            {
                var model = await _repoWrapper.UserRole.Get(id);

                var result = new RoleUpdateViewModel();
                if (model != null)
                {
                    result = new RoleUpdateViewModel
                    {
                        id = model.Id,
                        role = model.Role,
                        updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : ""
                    };
                }
                return Ok(new ApiResponse { data = result, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("User Role: Edit" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // PUT api/<UserRoleController>/save/5
        [HttpPut]
        [Route("Save/{id}")]
        public async Task<ActionResult<ApiResponse>> Save(int id, [FromBody] RoleUpdateViewModel model)
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
                    var role = await _repoWrapper.UserRole.Get(id);
                    role.Role = model.role;
                    role.IsActive = true;
                    role.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                    role.UpdatedDate = DateTime.Now;
                    await _repoWrapper.UserRole.Update(role);

                }
                else
                {
                    var role = new UserRole
                    {
                        Role = model.role,
                        IsActive = true,
                        UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null,
                        UpdatedDate = DateTime.Now
                    };
                    role = await _repoWrapper.UserRole.Add(role);
                    model.id = role.Id;
                }
                return Ok(new ApiResponse { data = model, message = "Successfully " + (id <= 0 ? "Created" : "Updated"), sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("UserRole: Save" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // DELETERESTORE api/<UserRoleController>/deleterestore/5
        [HttpDelete]
        [Route("DeleteRestore/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteRestore(int id, [FromBody] bool isDelete)
        {
            try
            {
                var model = await _repoWrapper.UserRole.Get(id);
                if (model == null)
                {
                    return NotFound(new ApiResponse { errors = "Item Not Found", message = "Could not found Item", sessionTimeout = sessionTimeout });
                }

                model.IsActive = !isDelete;
                model.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                model.UpdatedDate = DateTime.Now;
                
                await _repoWrapper.UserRole.Update(model);

                var result = new RoleViewModel
                {
                    id = model.Id,
                    role = model.Role,
                    updatedBy = model.UpdatedBy != 0 ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : "",
                    updatedDate = model.UpdatedDate

                };
                return Ok(new ApiResponse { data = result, message = "Successfully " + (isDelete ? "Deleted" : "Restored"), sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("UserRole: DeleteRestore" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }
    }
}
