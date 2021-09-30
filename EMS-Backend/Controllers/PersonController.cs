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
    [RoleAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : BaseController
    {
        public PersonController(IRepositoryWrapper repoWrapper, ILogger<PersonController> logger, IConfiguration configuration)
        {
            _repoWrapper = repoWrapper;
            _logger = logger;
            _configuration = configuration;
            TimeSpan expiryHours = TimeSpan.FromHours(int.Parse(_configuration["JWT:ExpiryHours"]));
            sessionTimeout = expiryHours.TotalMinutes;

        }

        // POST: api/<PersonController>/list/true
        [HttpPost]
        [Route("List/{includeDeleted}")]
        public async Task<ActionResult<ApiResponse>> List(bool includeDeleted, [FromBody] ListSettings ls)
        {
            try
            {
                var persons = await _repoWrapper.Person.EfficientGetAll(includeDeleted, ls);
                var userList = await _repoWrapper.User.GetAll();


                var result = (from p in persons.Item1
                              join user in userList on p.UpdatedBy equals user.Id into u
                              from uu in u.DefaultIfEmpty()

                              select new PersonViewModel
                              {
                                  id = p.Id,
                                  fullName = GetPersonFullName(p),
                                  genderId = (Gender)p.Gender,
                                  dob = p.Dob,
                                  address = p.Address,
                                  barangay = p.Brgy.Name,
                                  cluster = p.Cluster.Name,
                                  precinct = p.Precinct.Name,
                                  school = p.School,
                                  //gCash = p.GcashNumber,
                                  votersId = p.VotersIdno,
                                  mobile = p.MobileNo,
                                  IsMember = (bool)p.IsMember,
                                  isActive = (bool)p.IsActive,
                                  emailaddress = p.EmailAddress,
                                  updatedBy = (uu == null ? String.Empty : GetUserFullName(uu)),
                                  updatedDate = p.UpdatedDate
                              }).ToList();
                return Ok(new ApiResponse { data = result, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Person: List" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }

        [HttpPost]
        [Route("ListApproved/{includeDeleted}")]
        public async Task<ActionResult<ApiResponse>> ListApproved(bool includeDeleted, [FromBody] ListSettings ls)
        {
            try
            {
                var persons = await _repoWrapper.Person.EfficientGetAllMemberStatus(includeDeleted, ls);
                var userList = await _repoWrapper.User.GetAll();


                var result = (from p in persons.Item1
                              join user in userList on p.UpdatedBy equals user.Id into u
                              from uu in u.DefaultIfEmpty()

                              select new PersonViewModel
                              {
                                  id = p.Id,
                                  fullName = GetPersonFullName(p),
                                  genderId = (Gender)p.Gender,
                                  dob = p.Dob,
                                  address = p.Address,
                                  barangay = p.Brgy.Name,
                                  cluster = p.Cluster.Name,
                                  precinct = p.Precinct.Name,
                                  school = p.School,
                                  //gCash = p.GcashNumber,
                                  votersId = p.VotersIdno,
                                  mobile = p.MobileNo,
                                  emailaddress = p.EmailAddress,
                                  IsMember = (p.MembershipStatus == 1 && (bool)p.IsMember),
                                  isActive = (bool)p.IsActive,
                                  updatedBy = (uu == null ? String.Empty : GetUserFullName(uu)),
                                  updatedDate = p.UpdatedDate
                              }).ToList();
                return Ok(new ApiResponse { data = result, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Person: List" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }

        [HttpPost]
        [Route("ListVoters/{includeDeleted}")]
        public async Task<ActionResult<ApiResponse>> ListVoters(bool includeDeleted, [FromBody]ListSettings ls)
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
                                  fullName = p.LastName + ", " + p.FirstName + " " + p.MiddleName,
                                  genderId = p.Gender != null ? (Gender)p.Gender : (Gender)0,
                                  dob = p.Dob,
                                  address = p.Address,
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
                return Ok(new ApiResponse { data = result, dataCount = voters.Item2,  sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Person: List" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException != null ? ex.InnerException.Message : "", sessionTimeout = sessionTimeout });
            }

        }

        [HttpPost]
        [Route("ListPending/{includeDeleted}")]
        public async Task<ActionResult<ApiResponse>> ListPending(bool includeDeleted, [FromBody] ListSettings ls)
        {
            try
            {
                var persons = await _repoWrapper.Person.EfficientGetAllPending(includeDeleted, ls);
                var userList = await _repoWrapper.User.GetAll();


                var result = (from p in persons.Item1
                              join user in userList on p.UpdatedBy equals user.Id into u
                              from uu in u.DefaultIfEmpty()

                              select new PersonViewModel
                              {
                                  id = p.Id,
                                  fullName = GetPersonFullName(p),
                                  genderId = (Gender)p.Gender,
                                  dob = p.Dob,
                                  address = p.Address,
                                  barangay = p.Brgy.Name,
                                  cluster = p.Cluster.Name,
                                  precinct = p.Precinct.Name,
                                  school = p.School,
                                  //gCash = p.GcashNumber,
                                  votersId = p.VotersIdno,
                                  mobile = p.MobileNo,
                                  emailaddress = p.EmailAddress,
                                  IsMember = (p.MembershipStatus == 1),
                                  isActive = (bool)p.IsActive,
                                  ApprovalId = (Guid)p.ApprovalId,
                                  updatedBy = (uu == null ? String.Empty : GetUserFullName(uu)),
                                  updatedDate = p.UpdatedDate
                              }).ToList();
                return Ok(new ApiResponse { data = result, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Person: List" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }



        // GET api/<PersonController>/detail/5
        [HttpGet]
        [Route("Detail/{id}")]
        public async Task<ActionResult<ApiResponse>> Detail(int id, [FromBody] string message)
        {
            try
            {
                var model = await _repoWrapper.Person.Get(id);
                if (model == null)
                {
                    return NotFound(new ApiResponse { errors = "Item Not Found", message = "Could not found Item", sessionTimeout = sessionTimeout });
                }

                var result = new PersonViewModel
                {
                    id = model.Id,
                    fullName = GetPersonFullName(model),
                    genderId = (Gender)model.Gender,
                    dob = model.Dob,
                    address = model.Address,
                    barangay = model.Brgy.Name,
                    cluster = model.Cluster.Name,
                    precinct = model.Precinct.Name,
                    school = model.School,
                    //gCash = model.GcashNumber,
                    votersId = model.VotersIdno,
                    voterId = model.VoterId  != null ? (int)model.VoterId : 0,
                    mobile = model.MobileNo,
                    emailaddress = model.EmailAddress,
                    IsMember = (bool)model.IsMember,
                    isActive = (bool)model.IsActive,
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

        // GET api/<PersonController>/edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<ActionResult<ApiResponse>> Edit(int id)
        {
            try
            {
                var model = await _repoWrapper.Person.GetById(id);
                var genderList = InitEnums.GetListOfGender();


                var result = new PersonUpdateViewModel();

                if (model != null)
                {
                    result = new PersonUpdateViewModel
                    {
                        id = model.Id,
                        firstName = model.FirstName,
                        middleName = model.MiddleName,
                        lastName = model.LastName,
                        genderId = (Gender)model.Gender,
                        dob = model.Dob,
                        address = model.Address,
                        barangayId = model.Brgy.Id,
                        barangay = model.Brgy.Name,
                        clusterId = model.Cluster.Id,
                        cluster = model.Cluster.Name,
                        precinct = model.Precinct.Name,
                        precinctId = model.Precinct.Id,
                        school = model.School,
                        gCash = model.GcashNumber ?? string.Empty,
                        votersId = model.VotersIdno,
                        voterId = model.VoterId != null ? (int)model.VoterId : 0,
                        VoterFullname = model.Voter != null ? $"{model.Voter.LastName.ToUpper() }, {model.Voter.FirstName.ToUpper()} {model.Voter.MiddleName.ToUpper() }" : string.Empty,
                        VoterNo = model.Voter != null ? model.Voter.VotersIdno : string.Empty,
                        mobile = model.MobileNo,
                        emailaddress = model.EmailAddress,
                        ApprovalId = (Guid)model.ApprovalId,
                        MemberStatus = model.MembershipStatus,
                        updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : ""
                    };
                }

                return Ok(new ApiResponse { data = result, supportingLists = new { gender = genderList }, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Person: Edit" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // PUT api/<PersonController>/save/5
        [HttpPut]
        [Route("Save/{id}")]
        public async Task<ActionResult<ApiResponse>> Save(int id, [FromBody] PersonUpdateViewModel model)
        {
            var exPerson = _repoWrapper.Person.GetLastNameFirstName(model.firstName, model.lastName).GetAwaiter().GetResult();

            if (exPerson != null && id == 0)
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
                var personExist = await _repoWrapper.Person.Get(id);

                if (id != model.id)
                {
                    return BadRequest(new ApiResponse { errors = "Bad Request", message = "Bad Request", sessionTimeout = sessionTimeout });
                }

                if (personExist != null)
                {
                    var person = await _repoWrapper.Person.Get(id);
                    person.FirstName = model.firstName;
                    person.MiddleName = model.middleName;
                    person.LastName = model.lastName;
                    person.Gender = (int)model.genderId;
                    person.Dob = model.dob;
                    person.Address = model.address;
                    person.Brgy = await _repoWrapper.Barangay.Get(model.barangayId);
                    person.Cluster = await _repoWrapper.Cluster.Get(model.clusterId);
                    person.Precinct = await _repoWrapper.Precinct.Get(model.precinctId);
                    person.School = model.school;
                    person.GcashNumber = model.gCash ?? string.Empty;
                    person.VotersIdno = model.votersId;
                    person.MobileNo = model.mobile;
                    person.IsMember = model.IsMember;
                    person.EmailAddress = model.emailaddress;
                    person.IsActive = true;
                    person.VoterId = model.voterId;
                    person.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                    person.UpdatedDate = DateTime.Now;
                    //temporary set to false all;


                    await _repoWrapper.Person.Update(person);
                }
                else
                {
                    var person = new Person
                    {
                        FirstName = model.firstName,
                        MiddleName = model.middleName,
                        LastName = model.lastName,
                        Gender = (int)model.genderId,
                        Dob = model.dob,
                        Address = model.address,
                        Brgy = await _repoWrapper.Barangay.Get(model.barangayId),
                        Cluster = await _repoWrapper.Cluster.Get(model.clusterId),
                        Precinct = await _repoWrapper.Precinct.Get(model.precinctId),
                        School = model.school,
                        GcashNumber = model.gCash ?? string.Empty,
                        VotersIdno = model.votersId,
                        MobileNo = model.mobile,
                        IsMember = model.IsMember,
                        IsActive = true,
                        EmailAddress = model.emailaddress,
                        UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null,
                        UpdatedDate = DateTime.Now,
                        MembershipStatus = 0,
                        ApprovalId = Guid.NewGuid()
                    };

                    person = await _repoWrapper.Person.Add(person);

                    model.id = person.Id;
                }
                return Ok(new ApiResponse { data = model, message = "Successfully " + (id <= 0 ? "Created" : "Updated"), sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Person: Save" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // DELETE api/<PersonController>/delete/5
        [HttpDelete]
        [Route("DeleteRestore/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteRestore(int id, [FromBody] bool isDelete)
        {
            try
            {
                var model = await _repoWrapper.Person.GetById(id);
                if (model == null)
                {
                    return NotFound(new ApiResponse { errors = "Item Not Found", message = "Could not found Item", sessionTimeout = sessionTimeout });
                }

                model.IsActive = !isDelete;
                model.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                model.UpdatedDate = DateTime.Now;

                await _repoWrapper.Person.Update(model);

                var result = new PersonViewModel
                {
                    id = model.Id,
                    fullName = GetPersonFullName(model),
                    address = model.Address,
                    barangay = model.Brgy.Name,
                    cluster = model.Cluster.Name,
                    genderId = (Gender)model.Gender,
                    //gCash = model.GcashNumber,
                    precinct = model.Precinct.Name,
                    votersId = model.VotersIdno,
                    mobile = model.MobileNo,
                    dob = model.Dob,
                    school = model.School,
                    emailaddress = model.EmailAddress,
                    IsMember = (bool)model.IsMember,
                    isActive = (bool)model.IsActive,
                    updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : "",
                    updatedDate = model.UpdatedDate
                };

                return Ok(new ApiResponse { data = result, message = "Successfully Deleted", sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Person: Delete" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }


        [HttpPut]
        [Route("Approve/{id}")]
        public async Task<ActionResult<ApiResponse>> Approve(int id, [FromBody] PersonUpdateViewModel model)
        {
            try
            {
                var personModel = await _repoWrapper.Person.Get(id);
                var voter = await _repoWrapper.Voter.Get(model.voterId);
                var userRole = await _repoWrapper.UserRole.GetByName("member");
                var smsResult = new object();
                var result = new object();
                if (personModel != null)
                {
                    personModel.MembershipStatus = (int)VoterStatus.Approved;
                    personModel.Voter = voter;
                    personModel.VoterId = model.voterId;
                    await _repoWrapper.Person.Update(personModel);

                    User _newUser = new User();
                    string PasswordHash = BC.HashPassword("ems07112021");

                    _newUser.UserName = personModel.FirstName.ToLower() + "." + personModel.LastName;
                    _newUser.Email = "";
                    _newUser.Password = PasswordHash;
                    _newUser.IsActive = true;
                    _newUser.PersonId = personModel.Id;
                    _newUser.UserRoleId = userRole.Id;
                    var _userAccount = await _repoWrapper.User.Add(_newUser);

                    ActivationToken _actToken = new ActivationToken();
                    _actToken.Person = personModel;
                    _actToken.PersonId = personModel.Id;
                    _actToken.ExpireDate = DateTime.Now.AddHours(24);
                    string _approvalId = Guid.NewGuid().ToString();//.Split('-')[0];
                    _actToken.Token = _approvalId;
                    _actToken.IsActive = (bool)true;
                    var resultToken = await _repoWrapper.ActivationToken.Add(_actToken);

                    // Email
                    string url = $"{_configuration["Email:ApiURL"]}?approvalId={_approvalId.Split('-')[0]}";

                    // SMS
                    smsResult = EMS_SMS.SendOTP(model.mobile, _configuration["SMS:ApiCode"], _configuration["SMS:ApiPassword"], url);
                    _logger.LogInformation($" SMS Log: {smsResult}");

                    if (model.SMTP)
                    {
                        result = EMS_SMTP.SendActivationCode(personModel.FirstName.ToLower() + " " + personModel.LastName, url, _configuration["Email:EmailAddress"], _configuration["Email:Alias"], _configuration["Email:Password"], model.emailaddress);
                        _logger.LogInformation($" SMTP Log: {result}");
                    }
                    

                }

                return Ok(new ApiResponse { data = new { status = "approved", smtpLog = $" SMTP Log: {result}", smsLog = $" SMS Log: {smsResult}" }, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Person: Edit" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }


        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult<ApiResponse>> Add([FromBody] PersonRequest model)
        {
            try
            {
                var person = new Person
                {
                    FirstName = model.firstName,
                    MiddleName = model.middleName,
                    LastName = model.lastName,
                    Gender = (int)model.genderId,
                    Dob = model.dob,
                    Address = model.address,
                    Brgy = await _repoWrapper.Barangay.Get(model.barangayId),
                    Cluster = await _repoWrapper.Cluster.Get(model.clusterId),
                    Precinct = await _repoWrapper.Precinct.Get(model.precinctId),
                    School = model.school,
                    GcashNumber = model.gCash,
                    VotersIdno = model.votersId,
                    MobileNo = model.mobile,
                    EmailAddress = model.emailaddress,
                    IsMember = model.IsMember,
                    IsActive = true,
                    UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null,
                    UpdatedDate = DateTime.Now,
                    MembershipStatus = 0,
                    ApprovalId = Guid.NewGuid()
                };

                person = await _repoWrapper.Person.Add(person);

                return StatusCode(StatusCodes.Status201Created, new ApiResponse { data = model, message = "Successfully Created", sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Person: Edit" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });

            }

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
