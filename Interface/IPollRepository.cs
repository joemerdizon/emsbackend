using Entities.Models;
using Entities.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface
{
    public interface IPollRepository : IRepositoryBase<Poll>
    {
        /// <summary>
        /// Lazy loading of list due to large no of Data
        /// </summary>        
        /// <param name="ls">ListSetting provided by the view</param>
        /// <returns></returns>
        Task<(List<Poll>, int)> EfficientGetAll(ListSettings ls);

    }
}
