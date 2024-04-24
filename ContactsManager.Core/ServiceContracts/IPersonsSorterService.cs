using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person Entity
    /// </summary>
    public  interface IPersonsSorterService
    {
       
        /// <summary>
        /// it returns sorted list of persons
        /// </summary>
        /// <param name="allPersons">represents list of persons to be sorted</param>
        /// <param name="sortBy">name of the property (key), based on which persons should be sorted </param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>returns sorted persons as PersonResponse list</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons,string sortBy,SortOrderOptions sortOrder);
       
    }
}
 