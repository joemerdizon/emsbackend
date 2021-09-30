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
    [RoleAuthorize("superadmin, admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PolicyController : BaseController
    {
        public PolicyController(IRepositoryWrapper repoWrapper, ILogger<PolicyController> logger, IConfiguration configuration)
        {
            _repoWrapper = repoWrapper;
            _logger = logger;
            _configuration = configuration;
            TimeSpan expiryHours = TimeSpan.FromHours(int.Parse(_configuration["JWT:ExpiryHours"]));
            sessionTimeout = expiryHours.TotalMinutes;

        }

        // GET: api/<PolicyController>/list
        [HttpGet]
        [Route("List/{includeDeleted}")]
        public async Task<ActionResult<ApiResponse>> List(bool includeDeleted)
        {
            try
            {
                var policies = await _repoWrapper.Policy.GetAll();
                var userList = await _repoWrapper.User.GetAll();
                policies = includeDeleted ? policies : policies.Where(x => x.IsActive == true).ToList();
                var result = (from policy in policies
                              join user in userList on policy.UpdatedBy equals user.Id into u
                              from uu in u.DefaultIfEmpty()

                              select new PolicyViewModel
                              {
                                  id = policy.Id,
                                  name = policy.Name,
                                  description = policy.Description,
                                  assignedRoles = string.Join(", ", policy.PolicyRoles.Where(x=> x.IsActive == true).Select(X => X.Role.Role)),
                                  isActive = (bool)policy.IsActive,
                                  updatedBy = (uu == null ? String.Empty : GetUserFullName(uu)),
                                  updatedDate = policy.UpdatedDate
                              }).ToList();

                return Ok(new ApiResponse { data = result, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Policy: List" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }
        // GET api/<PolicyController>/detail/5
        [HttpGet]
        [Route("Detail/{id}")]
        public async Task<ActionResult<ApiResponse>> Detail(int id, [FromBody] string message)
        {
            try
            {
                var model = await _repoWrapper.Policy.Get(id);
                if (model == null)
                {
                    return NotFound(new ApiResponse { errors = "Item Not Found", message = "Could not found Item", sessionTimeout = sessionTimeout });
                }

                var result = new PolicyViewModel
                {
                    id = model.Id,
                    name = model.Name,
                    description = model.Description,
                    updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : "",
                    updatedDate = model.UpdatedDate
                };
                return Ok(new ApiResponse { data = result, message = message != null ? message : "", sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Policy: Detail" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // GET api/<PolicyController>/edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<ActionResult<ApiResponse>> Edit(int id)
        {
            try
            {
                var model = await _repoWrapper.Policy.Get(id);

                var userRoles = await _repoWrapper.UserRole.GetAll();
                var rolelist = (from role in userRoles.Where(x => x.IsActive == true)
                                select new { value = role.Id, text = role.Role }).ToList();

                var result = new PolicyUpdateViewModel();
                //Get All ModuleControl
                IList<ModuleControl> moduleControls = await _repoWrapper.ModuleControl.GetAll();

                if (model != null)
                {
                    result = new PolicyUpdateViewModel
                    {
                        id = model.Id,
                        name = model.Name,
                        description = model.Description,
                        policyRoles = model.PolicyRoles.Where(x => x.IsActive == true).Select(x => Convert.ToInt32(x.RoleId)).ToList(),
                        updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : ""
                    };
                }
                int currentModule = 0;

                foreach (var item in moduleControls)
                {
                    if (currentModule != item.ModuleId)
                    {
                        result.policyModule.Add(new PolicyModuleViewModel
                        {
                            moduleId = item.ModuleId,
                            module = item.Module.Name
                        });
                    }

                    if (model != null && model.PolicyModuleControl.FirstOrDefault(x => x.ModuleControlId == item.Id) != null)
                    {
                        var policyModuleControl = model.PolicyModuleControl.First(x => x.ModuleControlId == item.Id);

                        result.policyModule.Last().policyModuleControl.Add(new PolicyModuleControlViewModel
                        {
                            id = policyModuleControl.Id,
                            policyId = policyModuleControl.PolicyId,
                            isRestricted = policyModuleControl.IsRestricted,
                            isEnabled = item.IsChecked,
                            isActive = (bool)policyModuleControl.IsActive,
                            moduleControl = new ModuleControlViewModel { id = item.Id, controlId = (Control)item.ControlId, moduleId = item.ModuleId, isChecked = item.IsChecked }
                        });
                    }
                    else
                    {
                        result.policyModule.Last().policyModuleControl.Add(new PolicyModuleControlViewModel
                        {
                            id = 0,
                            policyId = model != null ? model.Id : 0,
                            isRestricted = false,
                            isEnabled = item.IsChecked,
                            isActive = true,
                            moduleControl = new ModuleControlViewModel { id = item.Id, controlId = (Control)item.ControlId, moduleId = item.ModuleId, isChecked = item.IsChecked }
                        });
                    }
                    currentModule = item.ModuleId;
                }

                return Ok(new ApiResponse { data = result, supportingLists = new { role = rolelist }, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Policy: Edit" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // PUT api/<PolicyController>/save/1
        [HttpPut]
        [Route("Save/{id}")]
        public async Task<ActionResult<ApiResponse>> Save(int id, [FromBody] PolicyUpdateViewModel model)
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

                //Update
                if (id > 0)
                {
                    var policy = await _repoWrapper.Policy.Get(id);
                    policy.Name = model.name;
                    policy.Description = model.description;
                    policy.IsActive = true;
                    policy.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                    policy.UpdatedDate = DateTime.Now;
                    //temporary set to false all;
                    foreach (var item in policy.PolicyRoles)
                    {
                        item.IsActive = false;
                    }
                    if (model.policyRoles != null)
                    {
                        foreach (var roleId in model.policyRoles)
                        {
                            var policyRole = policy.PolicyRoles.FirstOrDefault(x => x.RoleId == roleId) ?? new PolicyRoles();
                            policyRole.RoleId = roleId;
                            policyRole.IsActive = true;
                            policyRole.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                            policyRole.UpdatedDate = DateTime.Now;
                            policy.PolicyRoles.Add(policyRole);
                        }
                    }

                    foreach (var mod in model.policyModule)
                    {
                        foreach (var item in mod.policyModuleControl.OrderBy(x => x.moduleControl.id))
                        {
                            var pmc = policy.PolicyModuleControl.FirstOrDefault(x => x.Id == item.id) ?? new PolicyModuleControl();
                            pmc.PolicyId = policy.Id;
                            pmc.IsRestricted = item.isRestricted;
                            pmc.ModuleControlId = item.moduleControl.id;
                            pmc.IsActive = true;
                            pmc.UpdatedBy = policy.UpdatedBy;
                            pmc.UpdatedDate = DateTime.Now;
                            policy.PolicyModuleControl.Add(pmc);
                        }
                    }

                    await _repoWrapper.Policy.Update(policy);

                }
                //Create New
                else
                {
                    var policy = new Policy
                    {
                        Name = model.name,
                        Description = model.description,
                        IsActive = true,
                        UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null,
                        UpdatedDate = DateTime.Now
                    };

                    foreach (var roleId in model.policyRoles)
                    {
                        policy.PolicyRoles.Add(new PolicyRoles
                        {
                            RoleId = roleId,
                            IsActive = true,
                            UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null,
                            UpdatedDate = DateTime.Now
                        });
                    }
                    foreach (var mod in model.policyModule)
                    {
                        foreach (var item in mod.policyModuleControl.OrderBy(x=>x.moduleControl.id))
                        {
                            policy.PolicyModuleControl.Add(new PolicyModuleControl
                            {
                                Id = item.id,
                                IsRestricted = item.isRestricted,
                                ModuleControlId = item.moduleControl.id,
                                IsActive = true,
                                UpdatedBy = policy.UpdatedBy,
                                UpdatedDate = DateTime.Now
                            });
                        }
                    }

                    policy = await _repoWrapper.Policy.Add(policy);
                    model.id = policy.Id;
                }
                return Ok(new ApiResponse { data = model, message = "Successfully " + (id <= 0 ? "Created" : "Updated"), sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Module: Save" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // DELETE api/<PolicyController>/delete/1
        [HttpDelete]
        [Route("DeleteRestore/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteRestore(int id, [FromBody] bool isDelete)
        {
            try
            {
                var model = await _repoWrapper.Policy.Get(id);
                if (model == null)
                {
                    return NotFound(new ApiResponse { errors = "Item Not Found", message = "Could not found Item", sessionTimeout = sessionTimeout });
                }

                model.IsActive = !isDelete;
                model.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                model.UpdatedDate = DateTime.Now;

                model = await _repoWrapper.Policy.Update(model);

                var result = new PolicyViewModel
                {
                    id = model.Id,
                    name = model.Name,
                    description = model.Description,
                    updatedBy = model.UpdatedBy != 0 ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : "",
                    updatedDate = model.UpdatedDate
                };

                return Ok(new ApiResponse { data = result, message = "Successfully Deleted", sessionTimeout = sessionTimeout });
            }
            catch (Exception ex)
            {
                _logger.LogError("Mocule: Delete" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }
    }
}
