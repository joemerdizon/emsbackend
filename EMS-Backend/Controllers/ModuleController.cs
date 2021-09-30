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
    public class ModuleController : BaseController
    {
        public ModuleController(IRepositoryWrapper repoWrapper, ILogger<ModuleController> logger, IConfiguration configuration)
        {
            _repoWrapper = repoWrapper;
            _logger = logger;
            _configuration = configuration;
            TimeSpan expiryHours = TimeSpan.FromHours(int.Parse(_configuration["JWT:ExpiryHours"]));
            sessionTimeout = expiryHours.TotalMinutes;
        }

        // POST: api/<ModuleController>/list
        [HttpPost]
        [Route("List/{includeDeleted}")]
        public async Task<ActionResult<ApiResponse>> List(bool includeDeleted, [FromBody] ListSettings ls)
        {
            try
            {

                var modules = await _repoWrapper.Module.GetAll();
                var userList = await _repoWrapper.User.GetAll();
                modules = includeDeleted ? modules : modules.Where(x => x.IsActive == true).ToList();
                var result = (from module in modules
                              join user in userList on module.UpdatedBy equals user.Id into u
                              from uu in u.DefaultIfEmpty()

                              select new ModuleViewModel
                              {
                                  id = module.Id,
                                  name = module.Name,
                                  description = module.Description,
                                  parentModuleId = module.ParentModuleId,
                                  pageId = (Page)module.PageId,
                                  iconClass = module.IconClass ?? string.Empty,
                                  isActive = (bool)module.IsActive,
                                  updatedBy = (uu == null ? String.Empty : GetUserFullName(uu)),
                                  updatedDate = module.UpdatedDate
                              }).ToList();

                return Ok(new ApiResponse { data = result, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Module: List" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }

        // GET api/<ModuleController>/detail/5
        [HttpGet]
        [Route("Detail/{id}")]
        public async Task<ActionResult<ApiResponse>> Detail(int id, [FromBody] string message)
        {
            try
            {
                var model = await _repoWrapper.Module.Get(id);
                if (model == null)
                {
                    return NotFound(new ApiResponse { errors = "Item Not Found", message = "Could not found Item", sessionTimeout = sessionTimeout });
                }

                var result = new ModuleViewModel
                {
                    id = model.Id,
                    name = model.Name,
                    description = model.Description,
                    parentModuleId = model.ParentModuleId,
                    pageId = (Page)model.PageId,
                    iconClass = model.IconClass ?? string.Empty,
                    updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : "",
                    updatedDate = model.UpdatedDate
                };
                return Ok(new ApiResponse { data = result, message = message != null ? message : "", sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Module: Detail" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // GET api/<ModuleController>/edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<ActionResult<ApiResponse>> Edit(int id)
        {
            try
            {
                var model = await _repoWrapper.Module.Get(id);

                var parentModuleList = InitEnums.GetListOfParentModules();
                var pageList = InitEnums.GetListOfPages();

                var result = new ModuleUpdateViewModel();

                if (model != null)
                {
                    result = new ModuleUpdateViewModel
                    {
                        id = model.Id,
                        name = model.Name,
                        description = model.Description,
                        parentModuleId = model.ParentModuleId,
                        pageId = (Page)model.PageId,
                        iconClass = model.IconClass ?? string.Empty,
                        updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : ""
                    };
                    //Get All Module controls save on the Moduke
                    IList<ModuleControl> moduleControls = await _repoWrapper.ModuleControl.GetAllByModuleId(id);
                    var moduleControlViewModel = moduleControls.Select(x =>
                           new ModuleControlViewModel { id = x.Id, controlId = (Control)x.ControlId, moduleId = x.ModuleId, isChecked = x.IsChecked }).ToList();

                    var controlList = InitEnums.GetListOfModuleControlVM();
                    var unAvailableControlList = controlList.Where(x => !moduleControlViewModel.Select(y => y.controlId).Contains(x.controlId));
                    //Add Unavailable options to module control selections so they can check
                    foreach (var controlViewModel in unAvailableControlList)
                    {
                        controlViewModel.moduleId = id;
                        moduleControlViewModel.Add(controlViewModel);
                    }

                    result.controlList = moduleControlViewModel;
                }
                else
                {
                    result.controlList = InitEnums.GetListOfModuleControlVM();
                }
                return Ok(new ApiResponse { data = result, supportingLists = new { parentModule = parentModuleList, page = pageList }, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Module: Edit" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // PUT api/<ModuleController>/save/1
        [HttpPut]
        [Route("Save/{id}")]
        public async Task<ActionResult<ApiResponse>> Save(int id, [FromBody] ModuleUpdateViewModel model)
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
                    var module = await _repoWrapper.Module.Get(id);
                    module.Name = model.name;
                    module.Description = model.description;
                    module.ParentModuleId = model.parentModuleId;
                    module.PageId = (int)model.pageId;
                    module.IconClass = model.iconClass;
                    module.IsActive = true;
                    module.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                    module.UpdatedDate = DateTime.Now;

                    foreach (var item in model.controlList)
                    {
                        module.ModuleControl.Add(new ModuleControl
                        {
                            Id = item.id,
                            ControlId = (int)item.controlId,
                            ModuleId = module.Id,
                            IsChecked = item.isChecked,
                            IsActive = true,
                            UpdatedBy = module.UpdatedBy,
                            UpdatedDate = DateTime.Now
                        });
                    }

                    await _repoWrapper.Module.Update(module);

                }
                //Create New
                else
                {
                    var module = new Module
                    {
                        Name = model.name,
                        Description = model.description,
                        ParentModuleId = model.parentModuleId,
                        PageId = (int)model.pageId,
                        IconClass = model.iconClass,
                        IsActive = true,
                        UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null,
                        UpdatedDate = DateTime.Now
                    };

                    foreach (var item in model.controlList)
                    {
                        module.ModuleControl.Add(new ModuleControl
                        {
                            ControlId = (int)item.controlId,
                            IsChecked = item.isChecked,
                            IsActive = true,
                            UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null,
                            UpdatedDate = DateTime.Now
                        });
                    }
                    module = await _repoWrapper.Module.Add(module);
                    model.id = module.Id;
                }
                return Ok(new ApiResponse { data = model, message = "Successfully " + (id <= 0 ? "Created" : "Updated"), sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Module: Save" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // DELETE api/<ModuleController>/delete/1
        [HttpDelete]
        [Route("DeleteRestore/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteRestore(int id, [FromBody] bool isDelete)
        {
            try
            {
                var model = await _repoWrapper.Module.Get(id);
                if (model == null)
                {
                    return NotFound(new ApiResponse { errors = "Item Not Found", message = "Could not found Item", sessionTimeout = sessionTimeout });
                }

                model.IsActive = !isDelete;
                model.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                model.UpdatedDate = DateTime.Now;

                model = await _repoWrapper.Module.Update(model);

                var result = new ModuleViewModel
                {
                    id = model.Id,
                    name = model.Name,
                    description = model.Description,
                    parentModuleId = model.ParentModuleId,
                    pageId = (Page)model.PageId,
                    iconClass = model.IconClass ?? string.Empty,
                    updatedBy = model.UpdatedBy != 0 ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : "",
                    updatedDate = model.UpdatedDate
                };

                return Ok(new ApiResponse { data = result, message = "Successfully Deleted", sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Mocule: Delete" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }
    }
}
