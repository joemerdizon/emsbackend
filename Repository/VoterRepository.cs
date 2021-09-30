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
    public class VoterRepository : RepositoryBase<Voter>, IVoterRepository
    {
        public VoterRepository(EMSDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async override Task<List<Voter>> GetAll()
        {
            return await _dbContext.Voter.Include(d => d.Brgy).ThenInclude(x=>x.Zone).ThenInclude(m=>m.District)
                                         .Include(d => d.Cluster)
                                         .Include(d=>d.Precinct)
                                         .ToListAsync();
        }

        public async override Task<Voter> Get(int id)
        {
            return await _dbContext.Voter.Include(d => d.Brgy).ThenInclude(x => x.Zone).ThenInclude(m => m.District)
                .Include(d => d.Cluster).Include(d => d.Precinct)
                .SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async Task<(List<Voter>, int)> EfficientGetAll(bool includeDeleted, ListSettings ls)
        {
            var query = (from p in _dbContext.Voter.Include(d => d.Brgy).ThenInclude(x => x.Zone).ThenInclude(m => m.District)
                                         .Include(d => d.Cluster)
                                         .Include(d => d.Precinct)
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
            var count = await query.CountAsync();
            query = query.Skip(ls.skip).Take(ls.pageSize);

            return (await query.ToListAsync(), count);
        }

        public async Task<(List<Voter>, int)> EfficientGetAllWithoutPaging()
        {
            var query = (from p in _dbContext.Voter.Include(d => d.Brgy).ThenInclude(x => x.Zone).ThenInclude(m => m.District)
                                         .Include(d => d.Cluster)
                                         .Include(d => d.Precinct)
                         select p);
            
            var count = await query.CountAsync();
            return (await query.ToListAsync(), count);
        }

        public async Task<(List<Voter>, int)> EfficientGetAllSelection(bool includeDeleted, ListSettings ls, List<int> ids)
        {
            var query = (from p in _dbContext.Voter.Include(d => d.Brgy).ThenInclude(x => x.Zone).ThenInclude(m => m.District)
                                         .Include(d => d.Cluster)
                                         .Include(d => d.Precinct)
                         select p);
            //Show only active items'
            if (includeDeleted == false)
                query = query.Where(x => x.IsActive == true);// && !ids.Contains(x.Id));

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

        public async Task<Voter> GetLastNameFirstName(string lastName, string firstName)
        {
            return await _dbContext.Voter.Include(d => d.Brgy).SingleOrDefaultAsync(x => x.FirstName.Equals(firstName) && x.LastName.Equals(lastName));
        }
    }
}
