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

namespace EMS_Backend.Controllers
{
    [RoleAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VoterController : BaseController
    {
        public VoterController(IRepositoryWrapper repoWrapper, ILogger<VoterController> logger, IConfiguration configuration)
        {
            _repoWrapper = repoWrapper;
            _logger = logger;
            _configuration = configuration;
            TimeSpan expiryHours = TimeSpan.FromHours(int.Parse(_configuration["JWT:ExpiryHours"]));
            sessionTimeout = expiryHours.TotalMinutes;

        }

        // POST: api/<VoterController>/list/true
        [HttpPost]
        [Route("List/{includeDeleted}")]
        public async Task<ActionResult<ApiResponse>> List(bool includeDeleted, [FromBody] ListSettings ls)
        {
            try
            {
                var voters = await _repoWrapper.Voter.EfficientGetAll(includeDeleted, ls);
                var userList = await _repoWrapper.User.GetAll();


                var result = (from p in voters.Item1
                              join user in userList on p.UpdatedBy equals user.Id into u
                              from uu in u.DefaultIfEmpty()

                              select new VoterViewModel
                              {
                                  id = p.Id,
                                  fullName = GetVoterFullName(p),
                                  genderId = p.Gender != null ? (Gender)p.Gender : (Gender)0,
                                  dob = p.Dob,
                                  address = p.Address,
                                  district = p.Brgy.Zone.District.Name,
                                  zone = p.Brgy.Zone.Name,
                                  barangay = p.Brgy.Name,
                                  cluster = p.Cluster.Name,
                                  precinct = p.Precinct.Name,
                                  school = p.School,
                                  votersId = p.VotersIdno,
                                  mobile = p.MobileNo ?? string.Empty,
                                  isActive = (bool)p.IsActive,
                                  updatedBy = (uu == null ? String.Empty : GetUserFullName(uu)),
                                  updatedDate = p.UpdatedDate
                              }).ToList();
                return Ok(new ApiResponse { data = result, dataCount = voters.Item2, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Voter: List" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }

        // GET api/<VoterController>/detail/5
        [HttpGet]
        [Route("Detail/{id}")]
        public async Task<ActionResult<ApiResponse>> Detail(int id, [FromBody] string message)
        {
            try
            {
                var model = await _repoWrapper.Voter.Get(id);
                if (model == null)
                {
                    return NotFound(new ApiResponse { errors = "Item Not Found", message = "Could not found Item", sessionTimeout = sessionTimeout });
                }

                var result = new VoterViewModel
                {
                    id = model.Id,
                    fullName = GetVoterFullName(model),
                    genderId = model.Gender != null ? (Gender)model.Gender : 0,
                    dob = model.Dob,
                    address = model.Address,
                    district = model.Brgy.Zone.District.Name,
                    zone = model.Brgy.Zone.Name,
                    barangay = model.Brgy.Name,
                    cluster = model.Cluster.Name,
                    precinct = model.Precinct.Name,
                    school = model.School,
                    votersId = model.VotersIdno,
                    mobile = model.MobileNo,
                    isActive = (bool)model.IsActive,
                    updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : "",
                    updatedDate = model.UpdatedDate
                };
                return Ok(new ApiResponse { data = result, message = message != null ? message : "", sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Voter: Detail" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // GET api/<VoterController>/edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<ActionResult<ApiResponse>> Edit(int id)
        {
            try
            {
                var model = await _repoWrapper.Voter.Get(id);
                var genderList = InitEnums.GetListOfGender();

                var result = new VoterUpdateViewModel();

                if (model != null)
                {
                    result = new VoterUpdateViewModel
                    {
                        id = model.Id,
                        firstName = model.FirstName,
                        middleName = model.MiddleName,
                        lastName = model.LastName,
                        genderId = model.Gender != null ? (Gender)model.Gender : 0,
                        dob = model.Dob,
                        address = model.Address,
                        districtId  = model.Brgy.Zone.DistrictId,
                        zoneId = model.Brgy.ZoneId,
                        brgyId = (int)model.BrgyId,
                        clusterId = (int)model.ClusterId,
                        precinctId = (int)model.PrecinctId,
                        school = model.School,
                        votersId = model.VotersIdno,
                        mobile = model.MobileNo ?? string.Empty,
                        updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : ""
                    };
                }

                return Ok(new ApiResponse { data = result, supportingLists = new { gender = genderList}, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Voter: Edit" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // PUT api/<VoterController>/save/5
        [HttpPut]
        [Route("Save/{id}")]
        public async Task<ActionResult<ApiResponse>> Save(int id, [FromBody] VoterUpdateViewModel model)
        {
            var exVoter = _repoWrapper.Voter.GetLastNameFirstName(model.firstName, model.lastName).GetAwaiter().GetResult();

            if (exVoter != null && id == 0)
            {
                ModelState.AddModelError("Firstname", "Member already exists");
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
                var VoterExist = await _repoWrapper.Voter.Get(id);

                if (id != model.id)
                {
                    return BadRequest(new ApiResponse { errors = "Bad Request", message = "Bad Request", sessionTimeout = sessionTimeout });
                }

                if (VoterExist != null)
                {
                    var Voter = await _repoWrapper.Voter.Get(id);
                    Voter.FirstName = model.firstName;
                    Voter.MiddleName = model.middleName;
                    Voter.LastName = model.lastName;
                    Voter.Gender = (int)model.genderId;
                    Voter.Dob = model.dob;
                    Voter.Address = model.address;
                    Voter.Brgy = await _repoWrapper.Barangay.Get(model.brgyId);
                    Voter.Cluster = await _repoWrapper.Cluster.Get(model.clusterId);
                    Voter.Precinct = await _repoWrapper.Precinct.Get(model.precinctId);
                    Voter.School = model.school;
                    Voter.VotersIdno = model.votersId;
                    Voter.MobileNo = model.mobile;
                    Voter.IsActive = true;
                    Voter.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                    Voter.UpdatedDate = DateTime.Now;
                    //temporary set to false all;


                    await _repoWrapper.Voter.Update(Voter);
                }
                else
                {
                    var Voter = new Voter
                    {
                        FirstName = model.firstName,
                        MiddleName = model.middleName,
                        LastName = model.lastName,
                        Gender = (int)model.genderId,
                        Dob = model.dob,
                        Address = model.address,
                        Brgy = await _repoWrapper.Barangay.Get(model.brgyId),
                        Cluster = await _repoWrapper.Cluster.Get(model.clusterId),
                        Precinct = await _repoWrapper.Precinct.Get(model.precinctId),
                        School = model.school,
                        VotersIdno = model.votersId,
                        MobileNo = model.mobile,
                        IsActive = true,
                        UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null,
                        UpdatedDate = DateTime.Now
                    };

                    Voter = await _repoWrapper.Voter.Add(Voter);

                    model.id = Voter.Id;
                }
                return Ok(new ApiResponse { data = model, message = "Successfully " + (id <= 0 ? "Created" : "Updated"), sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Voter: Save" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // DELETE api/<VoterController>/delete/5
        [HttpDelete]
        [Route("DeleteRestore/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteRestore(int id, [FromBody] bool isDelete)
        {
            try
            {
                var model = await _repoWrapper.Voter.Get(id);
                if (model == null)
                {
                    return NotFound(new ApiResponse { errors = "Item Not Found", message = "Could not found Item", sessionTimeout = sessionTimeout });
                }

                model.IsActive = !isDelete;
                model.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                model.UpdatedDate = DateTime.Now;

                await _repoWrapper.Voter.Update(model);

                var result = new VoterViewModel
                {
                    id = model.Id,
                    fullName = GetVoterFullName(model),
                    address = model.Address,
                    district = model.Brgy.Zone.District.Name,
                    zone = model.Brgy.Zone.Name,
                    barangay = model.Brgy.Name,
                    cluster = model.Cluster.Name,
                    precinct = model.Precinct.Name,
                    genderId = model.Gender != null ? (Gender)model.Gender : 0,
                    votersId = model.VotersIdno,
                    mobile = model.MobileNo,
                    dob = model.Dob,
                    school = model.School,
                    isActive = (bool)model.IsActive,
                    updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : "",
                    updatedDate = model.UpdatedDate
                };

                return Ok(new ApiResponse { data = result, message = "Successfully Deleted", sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Voter: Delete" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }
    }
}
