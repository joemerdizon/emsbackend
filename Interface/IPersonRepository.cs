using Entities.Models;
using Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface
{
    public interface IPersonRepository : IRepositoryBase<Person>
    {
        /// <summary>
        /// Lazy loading of list due to large no of Data
        /// </summary>
        /// <param name="includeDeleted"></param>
        /// <param name="ls">ListSetting provided by the view</param>
        /// <returns></returns>
        Task<(List<Person>, int)> EfficientGetAll(bool includeDeleted, ListSettings ls);

        /// <summary>
        /// Lazy loading of list due to large no of Data
        /// </summary>
        /// <param name="ls">ListSetting provided by the view</param>
        /// <returns></returns>
        Task<(List<Person>, int)> EfficientGetAllMembers(ListSettings ls);

        /// <summary>
        ///  Get Person lastname and firstname
        /// </summary>
        /// <param name="lastName"></param>
        /// <param name="firstName"></param>
        /// <returns></returns>
        Task<Person> GetLastNameFirstName(string lastName, string firstName);

        Task<(List<Person>, int)> EfficientGetAllMemberStatus(bool includeDisapproved, ListSettings ls);
        /// <summary>
        /// Get all Pending Applicationh and Deleted
        /// </summary>
        /// <param name="includeDeleted"></param>
        /// <param name="ls"></param>
        /// <returns></returns>
        Task<(List<Person>, int)> EfficientGetAllPending(bool includeDeleted, ListSettings ls);

        Task<PersonActivationRequest> GetPersonByToken(Guid token);
        Task<PersonActivationRequest> GetPersonByToken(string token);

        Task<Person> GetById(int id);

        Task<List<Person>> GetAllByBrgy(int brgyId);

        Task<(List<Voter>, int)> GetAllVoters(bool includeDeleted, ListSettings ls);

        Task<(List<Person>, int)> EfficientGetAllSelection(bool includeDeleted, ListSettings ls, List<int> ids);

        Task<(List<Person>, int)> EfficientGetAllWithoutPaging();
    }
}
