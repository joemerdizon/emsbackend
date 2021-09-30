using Entities.Models;
using Entities.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface
{
    public interface IVoterRepository : IRepositoryBase<Voter>
    {
        /// <summary>
        /// Lazy loading of list due to large no of Data
        /// </summary>
        /// <param name="includeDeleted"></param>
        /// <param name="ls">ListSetting provided by the view</param>
        /// <returns></returns>
        Task<(List<Voter>, int)> EfficientGetAll(bool includeDeleted, ListSettings ls);

        /// <summary>
        ///  Get Voter lastname and firstname
        /// </summary>
        /// <param name="lastName"></param>
        /// <param name="firstName"></param>
        /// <returns></returns>
        Task<Voter> GetLastNameFirstName(string lastName, string firstName);

        /// <summary>
        /// Get All Voters without paging and filter
        /// </summary>
        /// <returns></returns>
        Task<(List<Voter>, int)> EfficientGetAllWithoutPaging();
        /// <summary>
        /// Get All filtered Ids
        /// </summary>
        /// <param name="includeDeleted">always false</param>
        /// <param name="ls">list settings</param>
        /// <param name="ids">selected ids</param>
        /// <returns></returns>
        Task<(List<Voter>, int)> EfficientGetAllSelection(bool includeDeleted, ListSettings ls, List<int> ids);
    }
}
