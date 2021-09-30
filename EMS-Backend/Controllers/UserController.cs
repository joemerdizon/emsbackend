using System;
using System.Collections.Generic;
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
using BC = BCrypt.Net.BCrypt;

namespace EMS_Backend.Controllers
{
    [RoleAuthorize("superadmin, admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        public UserController(IRepositoryWrapper repoWrapper, ILogger<UserController> logger, IConfiguration configuration)
        {
            _repoWrapper = repoWrapper;
            _logger = logger;
            _configuration = configuration;
            TimeSpan expiryHours = TimeSpan.FromHours(int.Parse(_configuration["JWT:ExpiryHours"]));
            sessionTimeout = expiryHours.TotalMinutes;

        }

        // GET: api/<UserController>/list
        [HttpGet]
        [Route("List/{includeDeleted}")]
        public async Task<ActionResult<ApiResponse>> List(bool includeDeleted)
        {
            try
            {

                var users = await _repoWrapper.User.GetAll();
                var roles = await _repoWrapper.UserRole.GetAll();

                users = includeDeleted ? users : users.Where(x => x.IsActive == true).ToList();
                var result = (from user in users
                              join role in roles on user.UserRoleId equals role.Id
                              join user2 in users on user.UpdatedBy equals user2.Id into u
                              from uu in u.DefaultIfEmpty()

                              select new UserViewModel
                              {
                                  id = user.Id,
                                  username = user.UserName,
                                  email = user.Email,
                                  name = GetUserFullName(user),
                                  role = role.Role,
                                  isActive = (bool)user.IsActive,
                                  updatedBy = (uu == null ? String.Empty : GetUserFullName(uu)),
                                  updatedDate = user.UpdatedDate
                              }).ToList();
                return Ok(new ApiResponse { data = result, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("User: List" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }

        // GET api/<UserController>/detail/5
        [HttpGet]
        [Route("Detail/{id}")]
        public async Task<ActionResult<ApiResponse>> Detail(int id, [FromBody] string message)
        {
            try
            {
                var model = await _repoWrapper.User.Get(id);
                if (model == null)
                {
                    return NotFound(new ApiResponse { errors = "Item Not Found", message = "Could not found Item", sessionTimeout = sessionTimeout });
                }

                var result = new UserViewModel
                {
                    id = model.Id,
                    username = model.UserName,
                    email = model.Email,
                    name = GetUserFullName(model),
                    role = model.UserRole.Role,
                    profilePicture = model.ProfilePicture,
                    updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : "",
                    updatedDate = model.UpdatedDate
                };
                return Ok(new ApiResponse { data = result, message = message != null ? message : "", sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("User: Detail" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // GET api/<UserController>/edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<ActionResult<ApiResponse>> Edit(int id)
        {
            try
            {
                var model = await _repoWrapper.User.Get(id);
                var userRoles = await _repoWrapper.UserRole.GetAll();
                var rolelist = (from role in userRoles.Where(x => x.IsActive == true)
                                select new { value = role.Id, text = role.Role }).ToList();

                var result = new UserUpdateViewModel();

                if (model != null)
                {
                    result = new UserUpdateViewModel
                    {
                        id = model.Id,
                        username = model.UserName,
                        email = model.Email,
                        firstName = model.Person.FirstName,
                        middleName = model.Person.MiddleName,
                        lastName = model.Person.LastName,
                        profilePicture = model.ProfilePicture,
                        roleId = model.UserRoleId,
                        updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : ""
                    };
                }

                return Ok(new ApiResponse { data = result, supportingLists = new { role = rolelist}, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("User: Edit" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // PUT api/<UserController>/save/5
        [HttpPut]
        [Route("Save/{id}")]
        public async Task<ActionResult<ApiResponse>> Save(int id, [FromBody] UserUpdateViewModel model)
        {
            var exUser = _repoWrapper.User.GetByUserName(model.username).GetAwaiter().GetResult();

            if (exUser != null && id == 0)
            {
                ModelState.AddModelError("Username", "Username already exists");
            }

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
                var userExist = await _repoWrapper.User.Get(id);
                // hash password
                string PasswordHash = BC.HashPassword(model.password);

                if (id != model.id)
                {
                    return BadRequest(new ApiResponse { errors = "Bad Request", message = "Bad Request", sessionTimeout = sessionTimeout });
                }

                if (userExist != null)
                {
                    var user = await _repoWrapper.User.Get(id);
                    user.UserRoleId = model.roleId;
                    user.UserName = model.username;
                    user.Email = model.email;
                    if (model.password.Length > 0)
                    {
                        user.Password = PasswordHash;
                    }
                    user.PersonId = model.personId;
                    user.ProfilePicture = model.profilePicture;
                    user.IsActive = true;
                    user.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                    user.UpdatedDate = DateTime.Now;
                    //temporary set to false all;

 
                    await _repoWrapper.User.Update(user);
                }
                else
                {
                    var user = new User
                    {
                        UserRoleId = model.roleId,
                        UserName = model.username,
                        Email = model.email,
                        Password = PasswordHash,
                        PersonId = model.personId,
                        ProfilePicture = model.profilePicture,
                        IsActive = true,
                        UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null,
                        UpdatedDate = DateTime.Now
                    };
                    
                    user = await _repoWrapper.User.Add(user);

                    model.id = user.Id;
                    model.password = "";
                    model.confirmPassword = "";
                }
                return Ok(new ApiResponse { data = model, message = "Successfully " + (id <= 0 ? "Created" : "Updated"), sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("User: Save" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // DELETE api/<UserController>/delete/5
        [HttpDelete]
        [Route("DeleteRestore/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteRestore(int id, [FromBody] bool isDelete)
        {
            try
            {
                var model = await _repoWrapper.User.Get(id);
                if (model == null)
                {
                    return NotFound(new ApiResponse { errors = "Item Not Found", message = "Could not found Item", sessionTimeout = sessionTimeout });
                }

                model.IsActive = !isDelete;
                model.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                model.UpdatedDate = DateTime.Now;

                await _repoWrapper.User.Update(model);

                var result = new UserViewModel
                {
                    id = model.Id,
                    username = model.UserName,
                    email = model.Email,
                    name = GetUserFullName(model),
                    role = model.UserRole.Role,
                    profilePicture = model.ProfilePicture,
                    updatedBy = model.UpdatedBy != 0 ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : "",
                    updatedDate = model.UpdatedDate
                };

                return Ok(new ApiResponse { data = result, message = "Successfully Deleted", sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("User: Delete" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }
    }
}
