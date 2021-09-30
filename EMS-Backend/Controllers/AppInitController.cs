using EMS_Backend.Helper;
using Entities.Enums;
using Entities.ViewModel;
using Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EMS_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppInitController : BaseController
    {
        public AppInitController(IRepositoryWrapper repositoryWrapper, ILogger<AppInitController> logger, IConfiguration configuration)
        {
            _repoWrapper = repositoryWrapper;
            _logger = logger;
            _configuration = configuration;
        }
        // GET: api/<UserRoleController>/list
        [HttpGet]
        [Route("BaseConstants")]
        public async Task<ActionResult<ApiResponse>> BaseConstants()
        {
            try
            {
                var modules = await _repoWrapper.Module.GetAll();
                var userRoles = await _repoWrapper.UserRole.GetAll();
                var userList = await _repoWrapper.User.GetAll();

                modules = modules.Where(x => x.IsActive == true).ToList();

                var moduleList = (from module in modules
                                  select new ModuleViewModel
                                  {
                                      id = module.Id,
                                      name = module.Name,
                                      description = module.Description,
                                      parentModuleId = module.ParentModuleId,
                                      pageId = (Page)module.PageId,
                                      iconClass = module.IconClass ?? "fas fa-circle",
                                      isActive = (bool)module.IsActive
                                  }).ToList();

                var districts = await _repoWrapper.District.GetAll();
                var zones = await _repoWrapper.Zone.GetAll();
                var brgys = await _repoWrapper.Barangay.GetAll();
                var cluss = await _repoWrapper.Cluster.GetAll();
                var press = await _repoWrapper.Precinct.GetAll();
                var genderList = InitEnums.GetListOfGender();

                var districtList = (from district in districts
                                    select new { value = district.Id, text = district.Name }).ToList();
                var zoneList = (from zone in zones
                                select new { value = zone.Id, text = zone.Name, parentId = zone.DistrictId }).ToList();
                var brgyList = (from brgy in brgys
                                select new { value = brgy.Id, text = brgy.Name, parentId = brgy.ZoneId }).ToList();
                var clusList = (from clus in cluss
                                select new { value = clus.Id, text = clus.Name, parentId = clus.BrgyId }).ToList();
                var presList = (from pres in press
                                select new { value = pres.Id, text = pres.Name, parentId = pres.ClusterId }).ToList();

                return Ok(new ApiResponse { data = new { modules = moduleList, districts = districtList, zones = zoneList, brgys = brgyList, clusters = clusList, precincts = presList, genders = genderList } });
            }
            catch (Exception ex)
            {
                _logger.LogError("AppInit: BaseContants" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }
    }
}
