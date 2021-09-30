using Entities.Enums;
using Entities.Models;
using Entities.ViewModel;
using Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PollRepository : RepositoryBase<Poll>, IPollRepository
    {
        public PollRepository(EMSDBContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(List<Poll>, int)> EfficientGetAll(ListSettings ls)
        {
            var query = (from p in _dbContext.Poll
                         select p);
            //Show only active items'
            query = query.Where(x => x.IsActive == true);

            //Search
            if (!string.IsNullOrEmpty(ls.searchValue))
            {
                query = query.Where(x => x.PollDescription.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.ExpiryDate.ToString().ToLower().Contains(ls.searchValue.ToLower())
                );
            }

            //Sorting
            if (!string.IsNullOrEmpty(ls.sortColumn))
            {
                //Sort Columns here are the viewmodel field equivalent. In this case FullName
                switch (ls.sortColumn)
                {
                    case "PollDescription":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.PollDescription) : query.OrderByDescending(x => x.PollDescription);
                        break;
                    case "ExpiryDate":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.ExpiryDate) : query.OrderByDescending(x => x.ExpiryDate);
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
    }
}
