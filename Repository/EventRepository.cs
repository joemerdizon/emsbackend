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
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        public EventRepository(EMSDBContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(List<Event>, int)> EfficientGetAll(ListSettings ls)
        {
            var query = (from p in _dbContext.Event
                         select p);
            //Show only active items'
            query = query.Where(x => x.IsActive == true);

            //Search
            if (!string.IsNullOrEmpty(ls.searchValue))
            {
                query = query.Where(x => x.Name.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Description.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Location.ToLower().Contains(ls.searchValue.ToLower()) ||
                                    x.Date.ToString().ToLower().Contains(ls.searchValue.ToLower())
                );
            }

            //Sorting
            if (!string.IsNullOrEmpty(ls.sortColumn))
            {
                //Sort Columns here are the viewmodel field equivalent. In this case FullName
                switch (ls.sortColumn)
                {
                    case "Name":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
                        break;
                    case "Description":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Description) : query.OrderByDescending(x => x.Description);
                        break;
                    case "Location":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Location) : query.OrderByDescending(x => x.Location);
                        break;
                    case "Date":
                        query = ls.sortColumnDir == SortDirection.Ascending ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date);
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

        public async Task<Event> GetByName(string name)
        {
            return await _dbContext.Event.SingleOrDefaultAsync(u => u.Name.Equals(name));
        }
    }
}
