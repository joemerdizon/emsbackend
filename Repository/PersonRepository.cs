using Entities.Models;
using Entities.ViewModel;
using Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Entities.Enums;
using System;

namespace Repository
{
    public class PersonRepository : RepositoryBase<Person>, IPersonRepository
    {
        public PersonRepository(EMSDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async override Task<List<Person>> GetAll()
        {
            return await _dbContext.Person.Include(d => d.User).ToListAsync();
        }

        public async Task<List<Person>> GetAllByBrgy(int brgyId)
        {
            return await _dbContext.Person.Where(x => x.Brgy.Id == brgyId).Include(d => d.User).ToListAsync();
        }

        public async override Task<Person> Get(int id)
        {
            return await _dbContext.Person.Include(d => d.User).SingleOrDefaultAsync(x => x.Id.Equals(id));
        }
        
        public async Task<Person> GetById(int id)
        {
            // Include Barangay Cluster Precinct
            return await _dbContext.Person.Include(v => v.Voter).Include(d => d.User).Include(x => x.Brgy).Include(x => x.Cluster).Include(x => x.Precinct).SingleOrDefaultAsync(x => x.Id.Equals(id));
        }


        public async Task<(List<Person>, int)> EfficientGetAll(bool includeDeleted, ListSettings ls)
        {
            var query = (from p in _dbContext.Person.Include(x => x.Brgy).Include(x => x.Cluster).Include(x => x.Precinct)
                         select p);
            //Show only active items'
            if (includeDeleted == false)
                query = query.Where(x => x.IsActive == true);
            //Filter by Location Heirarchy
            if (ls.filterType != 0)
            {
                switch (ls.filterType)
                {
                    case 1:
                        query = query.Where(x => x.Brgy.Zone.DistrictId == ls.filterId);
                        break;
                    case 2:
                        query = query.Where(x => x.Brgy.ZoneId == ls.filterId);
                        break;
                    case 3:
                        query = query.Where(x => x.Brgy.Id == ls.filterId);
                        break;
                    case 4:
                        query = query.Where(x => x.ClusterId == ls.filterId);
                        break;
                    case 5:
                        query = query.Where(x => x.PrecinctId == ls.filterId);
                        break;
                    default:
                        break;
                }
            }
            //Search
            if (!string.IsNullOrEmpty(ls.searchValue))
            {
                query = query.Where(x => x.FirstName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.MiddleName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.LastName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Address.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Cluster.Code.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    (x.Gender == 1 ? "Male" : "Female").ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.GcashNumber.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Precinct.Code.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.VotersIdno.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.MobileNo.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.School.ToLower().Contains(ls.searchValue.ToLower())
                );
            }

            //Sorting
            if (!string.IsNullOrEmpty(ls.sortColumn))
            {
                //Sort Columns here are the viewmodel field equivalent. In this case FullName
                switch (ls.sortColumn)
                {
                    case "FullName":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.LastName).ThenBy(n => n.FirstName) : query.OrderByDescending(x => x.LastName).ThenBy(n => n.FirstName);
                        break;
                    case "Address":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Address) : query.OrderByDescending(x => x.Address);
                        break;
                    case "Barangay":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Brgy.Code) : query.OrderByDescending(x => x.Brgy.Code);
                        break;
                    case "Cluster":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Cluster) : query.OrderByDescending(x => x.Cluster);
                        break;
                    case "Gender":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Gender) : query.OrderByDescending(x => x.Gender);
                        break;
                    case "GCash":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.GcashNumber) : query.OrderByDescending(x => x.GcashNumber);
                        break;
                    case "Precinct":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Precinct) : query.OrderByDescending(x => x.Precinct);
                        break;
                    case "VotersId":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.VotersIdno) : query.OrderByDescending(x => x.VotersIdno);
                        break;
                    case "Mobile":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.MobileNo) : query.OrderByDescending(x => x.MobileNo);
                        break;
                    case "School":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.School) : query.OrderByDescending(x => x.School);
                        break;
                    default:
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                        break;
                }
            }
            var count = await query.CountAsync();
            query = query.Skip(ls.skip).Take(ls.pageSize);

            return (await query.ToListAsync(), count);
        }

        public async Task<(List<Person>, int)> EfficientGetAllMemberStatus(bool includeDeleted, ListSettings ls)
        {
            var query = (from p in _dbContext.Person.Include(x => x.Brgy).Include(x => x.Cluster).Include(x => x.Precinct)
                         select p);

            if (!includeDeleted)
            {
                query = query.Where(x => x.MembershipStatus == 1 && x.IsActive == true && x.Id != 1);

            }
            else
            {
                query = query.Where(x => x.MembershipStatus != 0 && x.Id != 1);
            }


            //Search
            if (!string.IsNullOrEmpty(ls.searchValue))
            {
                query = query.Where(x => x.FirstName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.MiddleName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.LastName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Address.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Cluster.Code.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    (x.Gender == 1 ? "Male" : "Female").ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.GcashNumber.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Precinct.Code.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.VotersIdno.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.MobileNo.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.School.ToLower().Contains(ls.searchValue.ToLower())
                );
            }

            //Sorting
            if (!string.IsNullOrEmpty(ls.sortColumn))
            {
                //Sort Columns here are the viewmodel field equivalent. In this case FullName
                switch (ls.sortColumn)
                {
                    case "FullName":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.LastName).ThenBy(n => n.FirstName) : query.OrderByDescending(x => x.LastName).ThenBy(n => n.FirstName);
                        break;
                    case "Address":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Address) : query.OrderByDescending(x => x.Address);
                        break;
                    case "Barangay":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Brgy.Code) : query.OrderByDescending(x => x.Brgy.Code);
                        break;
                    case "Cluster":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Cluster) : query.OrderByDescending(x => x.Cluster);
                        break;
                    case "Gender":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Gender) : query.OrderByDescending(x => x.Gender);
                        break;
                    case "GCash":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.GcashNumber) : query.OrderByDescending(x => x.GcashNumber);
                        break;
                    case "Precinct":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Precinct) : query.OrderByDescending(x => x.Precinct);
                        break;
                    case "VotersId":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.VotersIdno) : query.OrderByDescending(x => x.VotersIdno);
                        break;
                    case "Mobile":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.MobileNo) : query.OrderByDescending(x => x.MobileNo);
                        break;
                    case "School":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.School) : query.OrderByDescending(x => x.School);
                        break;
                    default:
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                        break;
                }
            }
            var count = await query.CountAsync();
            query = query.Skip(ls.skip).Take(ls.pageSize);

            return (await query.ToListAsync(), count);
        }

        public async Task<(List<Person>, int)> EfficientGetAllPending(bool includeDeleted, ListSettings ls)
        {
            var query = (from p in _dbContext.Person.Include(x => x.Brgy).Include(x => x.Cluster).Include(x => x.Precinct)
                         select p);
            if (!includeDeleted)
            {
                query = query.Where(x => x.MembershipStatus == 0 && x.IsActive == true && x.Id != 1);

            }
            else
            {
                query = query.Where(x => x.MembershipStatus == 0 && x.Id != 1);
            }


            //Search
            if (!string.IsNullOrEmpty(ls.searchValue))
            {
                query = query.Where(x => x.FirstName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.MiddleName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.LastName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Address.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Brgy.Name.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Cluster.Name.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Precinct.Name.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.EmailAddress.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    (x.Gender == 1 ? "Male" : "Female").ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.GcashNumber.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Precinct.Code.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.VotersIdno.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.MobileNo.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.School.ToLower().Contains(ls.searchValue.ToLower())
                );
            }

            //Sorting
            if (!string.IsNullOrEmpty(ls.sortColumn))
            {
                //Sort Columns here are the viewmodel field equivalent. In this case FullName
                switch (ls.sortColumn)
                {
                    case "FullName":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.LastName).ThenBy(n => n.FirstName) : query.OrderByDescending(x => x.LastName).ThenBy(n => n.FirstName);
                        break;
                    case "Address":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Address) : query.OrderByDescending(x => x.Address);
                        break;
                    case "Barangay":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Brgy.Code) : query.OrderByDescending(x => x.Brgy.Code);
                        break;
                    case "Cluster":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Cluster) : query.OrderByDescending(x => x.Cluster);
                        break;
                    case "Gender":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Gender) : query.OrderByDescending(x => x.Gender);
                        break;
                    case "EmailAddress":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.EmailAddress) : query.OrderByDescending(x => x.EmailAddress);
                        break;
                    case "GCash":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.GcashNumber) : query.OrderByDescending(x => x.GcashNumber);
                        break;
                    case "Precinct":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Precinct) : query.OrderByDescending(x => x.Precinct);
                        break;
                    case "VotersId":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.VotersIdno) : query.OrderByDescending(x => x.VotersIdno);
                        break;
                    case "Mobile":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.MobileNo) : query.OrderByDescending(x => x.MobileNo);
                        break;
                    case "School":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.School) : query.OrderByDescending(x => x.School);
                        break;
                    default:
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                        break;
                }
            }
            var count = await query.CountAsync();
            query = query.Skip(ls.skip).Take(ls.pageSize);

            return (await query.ToListAsync(), count);
        }

        public async Task<Person> GetLastNameFirstName(string lastName, string firstName)
        {
            return await _dbContext.Person.Include(d => d.User).SingleOrDefaultAsync(x => x.FirstName.Equals(firstName) && x.LastName.Equals(lastName));
        }


        public async Task<PersonActivationRequest> GetPersonByToken(Guid token)
        {
            var tokenActivation =   _dbContext.ActivationToken.Include(x => x.Person).SingleOrDefault(x => x.Token.ToLower() == token.ToString().ToLower() && x.IsActive == true);
            var resultModel = new PersonActivationRequest();

            if (tokenActivation != null)
            {
                if (tokenActivation.ExpireDate.Subtract(DateTime.Now).Seconds > 1)
                {
                    resultModel.message = "true";
                    resultModel.personId = tokenActivation.Person.Id;
                }
                else
                {
                    resultModel.message = "false";
                    resultModel.personId = tokenActivation.Person.Id;
                }
            }
            else
            {
                resultModel.message = "false";
                resultModel.personId = 0;
            }

            return resultModel;
        }


        public async Task<PersonActivationRequest> GetPersonByToken(string token)
        {
            var tokenActivation = _dbContext.ActivationToken.Include(x => x.Person).SingleOrDefault(x => x.Token.StartsWith(token) && x.IsActive == true);
            var resultModel = new PersonActivationRequest();

            if (tokenActivation != null)
            {
                if (tokenActivation.ExpireDate.Subtract(DateTime.Now).Seconds > 1)
                {
                    resultModel.message = "true";
                    resultModel.personId = tokenActivation.Person.Id;
                }
                else
                {
                    resultModel.message = "false";
                    resultModel.personId = tokenActivation.Person.Id;
                }
            }
            else
            {
                resultModel.message = "false";
                resultModel.personId = 0;
            }

            return resultModel;
        }



        public async Task<(List<Person>, int)> EfficientGetAllMembers(ListSettings ls)
        {
            var query = (from p in _dbContext.Person.Include(x => x.Brgy).Include(x => x.Cluster).Include(x => x.Precinct)
                         select p);

            query = query.Where(x => x.IsMember == true && x.IsActive == true);

            //Search
            if (!string.IsNullOrEmpty(ls.searchValue))
            {
                query = query.Where(x => x.FirstName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.MiddleName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.LastName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Address.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Cluster.Name.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.EmailAddress.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    (x.Gender == 1 ? "Male" : "Female").ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.GcashNumber.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Precinct.Name.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.VotersIdno.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.MobileNo.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.School.ToLower().Contains(ls.searchValue.ToLower())
                );
            }

            //Sorting
            if (!string.IsNullOrEmpty(ls.sortColumn))
            {
                //Sort Columns here are the viewmodel field equivalent. In this case FullName
                switch (ls.sortColumn)
                {
                    case "FullName":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.LastName).ThenBy(n => n.FirstName) : query.OrderByDescending(x => x.LastName).ThenBy(n => n.FirstName);
                        break;
                    case "Address":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Address) : query.OrderByDescending(x => x.Address);
                        break;
                    case "Barangay":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Brgy.Name) : query.OrderByDescending(x => x.Brgy.Name);
                        break;
                    case "Cluster":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Cluster) : query.OrderByDescending(x => x.Cluster);
                        break;

                    case "EmailAddress":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.EmailAddress) : query.OrderByDescending(x => x.EmailAddress);
                        break;
                    case "GCash":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.GcashNumber) : query.OrderByDescending(x => x.GcashNumber);
                        break;
                    case "Precinct":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Precinct) : query.OrderByDescending(x => x.Precinct);
                        break;
                    case "VotersId":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.VotersIdno) : query.OrderByDescending(x => x.VotersIdno);
                        break;
                    case "Mobile":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.MobileNo) : query.OrderByDescending(x => x.MobileNo);
                        break;
                    case "School":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.School) : query.OrderByDescending(x => x.School);
                        break;
                    case "Gender":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Gender) : query.OrderByDescending(x => x.Gender);
                        break;
                    default:
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                        break;
                }
            }
            var count = await query.CountAsync();
            query = query.Skip(ls.skip).Take(ls.pageSize);

            return (await query.ToListAsync(), count);
        }


        public async Task<(List<Voter>, int)> GetAllVoters(bool includeDeleted, ListSettings ls)
        {
            var query = (from p in _dbContext.Voter.Include(x => x.Brgy).Include(x => x.Precinct).Include(x => x.Cluster)
                         select p);
            var personVoterId = _dbContext.Person.Where(x => x.IsMember == true && x.MembershipStatus == 1).Select(x => x.VoterId).ToList();
            query = query.Where(x => x.IsActive == true && !personVoterId.Contains(x.Id));

            //Search
            if (!string.IsNullOrEmpty(ls.searchValue))
            {
                query = query.Where(x => x.FirstName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.MiddleName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.LastName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Address.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Cluster.Name.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    (x.Gender == 1 ? "Male" : "Female").ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Precinct.Name.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.VotersIdno.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.MobileNo.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.School.ToLower().Contains(ls.searchValue.ToLower())
                );
            }

            //Sorting
            if (!string.IsNullOrEmpty(ls.sortColumn))
            {
                //Sort Columns here are the viewmodel field equivalent. In this case FullName
                switch (ls.sortColumn)
                {
                    case "FullName":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.LastName).ThenBy(n => n.FirstName) : query.OrderByDescending(x => x.LastName).ThenBy(n => n.FirstName);
                        break;
                    case "Address":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Address) : query.OrderByDescending(x => x.Address);
                        break;
                    case "Barangay":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Brgy.Name) : query.OrderByDescending(x => x.Brgy.Name);
                        break;
                    case "Cluster":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Cluster) : query.OrderByDescending(x => x.Cluster);
                        break;
                    case "Gender":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Gender) : query.OrderByDescending(x => x.Gender);
                        break;
                    case "Precinct":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Precinct) : query.OrderByDescending(x => x.Precinct);
                        break;
                    case "VotersId":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.VotersIdno) : query.OrderByDescending(x => x.VotersIdno);
                        break;
                    case "Mobile":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.MobileNo) : query.OrderByDescending(x => x.MobileNo);
                        break;
                    case "School":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.School) : query.OrderByDescending(x => x.School);
                        break;
                    default:
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                        break;
                }
            }
            var count = await query.CountAsync();
            query = query.Skip(ls.skip).Take(ls.pageSize);

            return (await query.ToListAsync(), count);
        }

        public async Task<(List<Person>, int)> EfficientGetAllSelection(bool includeDeleted, ListSettings ls, List<int> ids)
        {
            var query = (from p in _dbContext.Person.Include(d => d.Brgy).ThenInclude(x => x.Zone).ThenInclude(m => m.District)
                                         .Include(d => d.Cluster)
                                         .Include(d => d.Precinct)
                         select p);
            //Show only active items'
            if (includeDeleted == false)
                query = query.Where(x => x.IsActive == true && x.IsMember == true);

            //Filter by Location Heirarchy
            if (ls.filterType != 0)
            {
                switch (ls.filterType)
                {
                    case 1:
                        query = query.Where(x => x.Brgy.Zone.DistrictId == ls.filterId);
                        break;
                    case 2:
                        query = query.Where(x => x.Brgy.ZoneId == ls.filterId);
                        break;
                    case 3:
                        query = query.Where(x => x.Brgy.Id == ls.filterId);
                        break;
                    case 4:
                        query = query.Where(x => x.ClusterId == ls.filterId);
                        break;
                    case 5:
                        query = query.Where(x => x.PrecinctId == ls.filterId);
                        break;
                    default:
                        break;
                }
            }

            if (ls.ageCondition != 0 && ls.age != 0)
            {
                switch (ls.ageCondition)
                {
                    case 1:
                        query = query.Where(x => (x.Dob != null ? DateTime.Now.Year - x.Dob.Value.Year : 0) <= ls.age);
                        break;
                    case 2:
                        query = query.Where(x => (x.Dob != null ? DateTime.Now.Year - x.Dob.Value.Year : 0) >= ls.age);
                        break;
                    default:
                        break;
                }
            }

            if (ls.gender != 0)
            {
                query = query.Where(x => x.Gender == ls.gender);
            }

            ////Search
            if (!string.IsNullOrEmpty(ls.searchValue))
            {
                query = query.Where(x => x.FirstName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.MiddleName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.LastName.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Address.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Cluster.Name.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    ((Gender)x.Gender).Equals(ls.searchValue.ToLower()) ||
                                    x.Brgy.Name.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Precinct.Name.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.VotersIdno.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.MobileNo.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.School.ToLower().Contains(ls.searchValue.ToLower())
                );
            }

            //Sorting
            if (!string.IsNullOrEmpty(ls.sortColumn))
            {
                //Sort Columns here are the viewmodel field equivalent. In this case FullName
                switch (ls.sortColumn)
                {
                    case "FullName":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.LastName).ThenBy(n => n.FirstName) : query.OrderByDescending(x => x.LastName).ThenBy(n => n.FirstName);
                        break;
                    case "Address":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Address) : query.OrderByDescending(x => x.Address);
                        break;
                    case "Barangay":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Brgy.Name) : query.OrderByDescending(x => x.Brgy.Name);
                        break;
                    case "Cluster":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Cluster) : query.OrderByDescending(x => x.Cluster);
                        break;
                    case "Gender":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Gender) : query.OrderByDescending(x => x.Gender);
                        break;
                    case "Precinct":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Precinct.Name) : query.OrderByDescending(x => x.Precinct.Name);
                        break;
                    case "VotersId":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.VotersIdno) : query.OrderByDescending(x => x.VotersIdno);
                        break;
                    case "Mobile":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.MobileNo) : query.OrderByDescending(x => x.MobileNo);
                        break;
                    case "School":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.School) : query.OrderByDescending(x => x.School);
                        break;
                    default:
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                        break;
                }
            }

            //Filter not yet selected
            if (ids.Count > 0)
                query = query.Where(x => !ids.Contains(x.Id));

            var count = await query.CountAsync();
            query = query.Skip(ls.skip).Take(ls.pageSize);

            return (await query.ToListAsync(), count);
        }

        public async Task<(List<Person>, int)> EfficientGetAllWithoutPaging()
        {
            var query = (from p in _dbContext.Person.Include(d => d.Brgy).ThenInclude(x => x.Zone).ThenInclude(m => m.District)
                                         .Include(d => d.Cluster)
                                         .Include(d => d.Precinct)
                         select p);

            query = query.Where(x => x.IsActive == true && x.IsMember == true);

            var count = await query.CountAsync();
            return (await query.ToListAsync(), count);
        }
    }
}
