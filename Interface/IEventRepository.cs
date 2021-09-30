using Entities.Models;
using Entities.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface
{
    public interface IEventRepository : IRepositoryBase<Event>
    {
        /// <summary>
        /// Lazy loading of list due to large no of Data
        /// </summary>        
        /// <param name="ls">ListSetting provided by the view</param>
        /// <returns></returns>
        Task<(List<Event>, int)> EfficientGetAll(ListSettings ls);

        /// <summary>
        /// Get Event records with name mathcing the parameter value
        /// </summary>
        /// <param name="name">name of event from input/validation</param>
        /// <returns></returns>
        Task<Event> GetByName(string name);
    }
}
