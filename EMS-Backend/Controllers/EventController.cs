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
    public class EventController : BaseController
    {
        public EventController(IRepositoryWrapper repoWrapper, ILogger<EventController> logger, IConfiguration configuration)
        {
            _repoWrapper = repoWrapper;
            _logger = logger;
            _configuration = configuration;
            TimeSpan expiryHours = TimeSpan.FromHours(int.Parse(_configuration["JWT:ExpiryHours"]));
            sessionTimeout = expiryHours.TotalMinutes;
        }

        // POST: api/<EventController>/list
        [HttpPost]
        [Route("List")]
        public async Task<ActionResult<ApiResponse>> List([FromBody] ListSettings ls)
        {
            try
            {
                var events = await _repoWrapper.Event.EfficientGetAll(ls);
                var userList = await _repoWrapper.User.GetAll();
                var result = (from even in events.Item1
                              join user in userList on even.CreatedBy equals user.Id into u
                              from uu in u.DefaultIfEmpty()

                              select new EventViewModel
                              {
                                  id = even.Id,
                                  name = even.Name,
                                  description = even.Description,
                                  typeOfEventId = (EventType)even.TypeOfEvent,
                                  location = even.Location ?? string.Empty,
                                  date = even.Date,
                                  isActive = (bool)even.IsActive,
                                  updatedBy = (uu == null ? String.Empty : GetUserFullName(uu)),
                                  updatedDate = even.UpdatedDate,
                                  isClosed = even.IsClosed != null ? even.IsClosed.Value : false
                              }).ToList();

                return Ok(new ApiResponse { data = result, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("EventManagement: List" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }

        // GET api/<EventController>/edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<ActionResult<ApiResponse>> Edit(int id)
        {
            try
            {
                var model = await _repoWrapper.Event.Get(id);
                var eventList = InitEnums.GetListOfEventTypes();
                var genderList = InitEnums.GetListOfGender();

                var result = new EventUpdateViewModel();
                

                if (model != null)
                {
                    IList<CashAidRecepients> details = await _repoWrapper.CashAidRecepient.GetAllByEventId(model.Id);

                    result = new EventUpdateViewModel
                    {
                        id = model.Id,
                        name = model.Name,
                        description = model.Description,
                        typeOfEventId = (EventType)model.TypeOfEvent,
                        additionalDetails = model.AdditionalDetails,
                        location = model.Location,
                        budget = model.Budget.Value,
                        date = model.Date,
                        cashAidAmount = model.TotalCashAid ?? 0,
                        additionalAmount = model.AdditionalBudget ?? 0,
                        runningCashAidAmount = model.RunningCashAid ?? 0,
                        runningAdditionalAmount = model.RunningAdditionalBudget ?? 0,
                        runningTotalAmount = model.RunningAmount ?? 0,
                        updatedBy = model.UpdatedBy != 0 && model.UpdatedBy != null ? GetUserFullName(await _repoWrapper.User.Get((int)model.UpdatedBy)) : "",
                        barangayId = model.TypeOfEvent == 1 && details.Any() ? details.First().Person.BrgyId.Value : 0,
                        isClosed = model.IsClosed.Value
                    };
                }

                return Ok(new ApiResponse { data = result, supportingLists = new { events = eventList, genders = genderList }, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("EventManagement: Edit" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        [HttpPut]
        [Route("Save/{id}")]
        public async Task<ActionResult<ApiResponse>> Save(int id, [FromBody] EventUpdateViewModel model)
        {
            var exName = _repoWrapper.Event.GetByName(model.name).GetAwaiter().GetResult();

            if (exName != null && id == 0)
            {
                ModelState.AddModelError("Name", "Name of event already exists, choose a different name.");
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
                var eventExist = await _repoWrapper.Event.Get(id);

                if (id != model.id)
                {
                    return BadRequest(new ApiResponse { errors = "Bad Request", message = "Bad Request", sessionTimeout = sessionTimeout });
                }

                if (eventExist != null)
                {
                    var even = await _repoWrapper.Event.Get(id);
                    even.Name = model.name;
                    even.Description = model.description;
                    even.Location = model.location;
                    even.AdditionalDetails = model.additionalDetails;                    
                    even.Date = model.date;
                    even.TypeOfEvent = (int)model.typeOfEventId;                    
                    even.IsActive = true;
                    even.UpdatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null;
                    even.UpdatedDate = DateTime.Now;
                    //even.TotalCashAid = (int)model.typeOfEventId == 1 ? model.cashAidAmount : 0;
                    //even.AdditionalBudget = (int)model.typeOfEventId == 1 ? model.additionalAmount : 0;
                    //even.Budget = model.budget;
                    even.RunningCashAid = (int)model.typeOfEventId == 1 ? model.runningCashAidAmount : 0;
                    even.RunningAdditionalBudget = (int)model.typeOfEventId == 1 ? model.runningAdditionalAmount : 0;
                    even.RunningAmount = model.runningTotalAmount;
                    even.IsClosed = model.isClosed;

                    await _repoWrapper.Event.Update(even);

                    if (model.cashAidNonVoters != null && model.cashAidNonVoters.Count > 0)
                    {
                        foreach (var nonV in model.cashAidNonVoters.Where(x => x.id == 0))
                        {
                            var nonVoter = new CashAidNonVoter
                            {
                                Event = even,
                                EventId = even.Id,
                                FirstName = nonV.firstName,
                                MiddleName = nonV.middleName,
                                LastName = nonV.lastName,
                                Dob = nonV.dob,
                                Address = nonV.address,
                                MobileNo = nonV.mobile,
                                Gender = nonV.genderName == "Unspecified" ? 0 : nonV.genderName == "Male" ? 1 : 2,
                                Brgy = await _repoWrapper.Barangay.Get(nonV.brgyId),
                                BrgyId = nonV.brgyId,
                                Amount = nonV.amount,
                                IsActive = true
                            };

                            nonVoter = await _repoWrapper.CashAidNonVoter.Add(nonVoter);
                        }
                    }

                    if (model.cashAidRecepients != null && model.cashAidRecepients.Count > 0)
                    {

                        var persons = await _repoWrapper.CashAidRecepient.GetAllByEventId(id);
                        var existing = persons != null ? persons.Select(x => x.PersonId).ToList() : new List<int> { 0 };

                        foreach (var nonV in model.cashAidRecepients.Where(x => x.id != 0 && !existing.Contains(x.id)))
                        {
                            var nonVoter = new CashAidRecepients
                            {
                                Event = even,
                                EventId = even.Id,
                                Person = await _repoWrapper.Person.Get(nonV.id),
                                PersonId = nonV.personId,
                                CreatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                PaymentType = 1,
                                Amount = nonV.amountPerHead
                            };

                            nonVoter = await _repoWrapper.CashAidRecepient.Add(nonVoter);
                        }
                    }
                }
                else
                {
                    var even = new Event
                    {
                        Name = model.name,
                        Description = model.description,
                        Location = model.location,
                        AdditionalDetails = model.additionalDetails,
                        Date = model.date,
                        TypeOfEvent = (int)model.typeOfEventId,
                        IsActive = true,
                        CreatedBy = GetCurrentUser() != "" ? (await _repoWrapper.User.GetByUserName(GetCurrentUser())).Id : (int?)null,
                        CreatedDate = DateTime.Now,
                        TotalCashAid = (int)model.typeOfEventId == 1 ? model.cashAidAmount : 0,
                        AdditionalBudget = (int)model.typeOfEventId == 1 ? model.additionalAmount : 0,
                        Budget = model.budget,
                        RunningCashAid = 0,
                        RunningAdditionalBudget = 0,
                        RunningAmount = 0,
                        IsClosed = false
                    };

                    even = await _repoWrapper.Event.Add(even);

                    model.id = even.Id;

                }
                return Ok(new ApiResponse { data = model, message = "Successfully " + (id <= 0 ? "Created" : "Updated"), sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("EventManagement: Save" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }

        // POST: api/<VoterController>/list/true
        [HttpPost]
        [Route("ListPerson")]
        public async Task<ActionResult<ApiResponse>> ListPerson(int id)
        {
            try
            {
                var persons = await _repoWrapper.CashAidRecepient.GetAllByEventId(id);                

                var result = (from p in persons

                              select new CashAidRecepientViewModel
                              {
                                  id = p.PersonId,
                                  fullName = GetPersonFullName(p.Person),
                                  genderId = p.Person.Gender != null ? (Gender)p.Person.Gender : (Gender)0,
                                  dob = p.Person.Dob,
                                  address = p.Person.Address,
                                  district = p.Person.Brgy.Zone.District.Name,
                                  zone = p.Person.Brgy.Zone.Name,
                                  barangay = p.Person.Brgy.Name,
                                  cluster = p.Person.Cluster.Name,
                                  precinct = p.Person.Precinct.Name,
                                  school = p.Person.School,
                                  votersId = p.Person.VotersIdno,
                                  mobile = p.Person.MobileNo ?? string.Empty,
                                  isActive = (bool)p.IsActive,
                                  updatedBy = "",
                                  updatedDate = p.UpdatedDate,
                                  age = p.Person.Dob != null ? DateTime.Now.Year - p.Person.Dob.Value.Year : 0,
                                  personId = p.PersonId,
                                  amountPerHead = p.Amount.Value
                              }).ToList();

                return Ok(new ApiResponse { data = result, dataCount = persons.Count(), sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Event: ListPerson" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }

        [HttpPost]
        [Route("ListPersonSelection")]
        public async Task<ActionResult<ApiResponse>> ListPersonSelection(int filterType, int filterId, int skip, int pageSize, List<int> ids)
        {
            try
            {
                var ls = new ListSettings();
                ls.filterType = filterType;
                ls.filterId = filterId;
                ls.skip = skip;
                ls.pageSize = pageSize;

                var persons = await _repoWrapper.Person.EfficientGetAllSelection(false, ls, ids);

                var result = (from p in persons.Item1

                              select new CashAidRecepientViewModel
                              {
                                  id = p.Id,
                                  fullName = GetPersonFullName(p),
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
                                  updatedBy = "",
                                  updatedDate = p.UpdatedDate,
                                  age = p.Dob != null ? DateTime.Now.Year - p.Dob.Value.Year : 0
                              }).ToList();

                return Ok(new ApiResponse { data = result, dataCount = persons.Item2, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Event: ListPerson" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }

        [HttpPost]
        [Route("ListPersonCreate")]
        public async Task<ActionResult<ApiResponse>> ListPersonCreate([FromBody] ListSettings ls)
        {
            try
            {
                var persons = await _repoWrapper.Voter.EfficientGetAll(false, ls);

                var result = (from p in persons.Item1

                              select new CashAidRecepientViewModel
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
                                  updatedBy = "",
                                  updatedDate = p.UpdatedDate,
                                  age = p.Dob != null ? DateTime.Now.Year - p.Dob.Value.Year : 0
                              }).ToList();

                return Ok(new ApiResponse { data = result, dataCount = persons.Item2, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Event: ListPerson" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }

        [HttpPost]
        [Route("ListPersonAll")]
        public async Task<ActionResult<ApiResponse>> ListPersonAll()
        {
            try
            {
                var persons = await _repoWrapper.Person.EfficientGetAllWithoutPaging();

                var result = (from p in persons.Item1

                              select new CashAidRecepientViewModel
                              {
                                  id = p.Id,
                                  fullName = GetPersonFullName(p),
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
                                  updatedBy = "",
                                  updatedDate = p.UpdatedDate,
                                  age = p.Dob != null ? DateTime.Now.Year - p.Dob.Value.Year : 0
                              }).ToList();

                return Ok(new ApiResponse { data = result, dataCount = persons.Item2, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Event: ListPerson" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }

        [HttpPost]
        [Route("ListNonVoter")]
        public async Task<ActionResult<ApiResponse>> ListNonVoter(int id)
        {
            try
            {
                var nonVoters = await _repoWrapper.CashAidNonVoter.GetAllByEventId(id);

                var result = (from p in nonVoters

                              select new CashAidNonVoterViewModel
                              {
                                  id = p.Id,
                                  fullName = p.LastName + ", " + p.FirstName + " " + p.MiddleName,
                                  firstName = p.FirstName,
                                  middleName = p.MiddleName,
                                  lastName = p.LastName,
                                  genderId = p.Gender != null ? (Gender)p.Gender : (Gender)0,
                                  dob = p.Dob,
                                  address = p.Address,
                                  barangay = p.Brgy.Name,
                                  cluster = "",                                  
                                  mobile = p.MobileNo ?? string.Empty,
                                  isActive = (bool)p.IsActive,
                                  updatedBy = "",
                                  updatedDate = p.UpdatedDate,
                                  age = p.Dob != null ? DateTime.Now.Year - p.Dob.Value.Year : 0,
                                  eventId = p.EventId,
                                  brgyId = p.BrgyId.Value,
                                  amount = p.Amount.Value
                              }).ToList();

                return Ok(new ApiResponse { data = result, dataCount = result.Count, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Event: ListNonVoter" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }

        }
    }
}
