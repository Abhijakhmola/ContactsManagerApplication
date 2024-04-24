using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person Entity
    /// </summary>
    public  interface IPersonsGetterService
    {
       
        Task<List<PersonResponse>> GetAllPersons();
        /// <summary>
        /// returns the person object based on the given person id
        /// </summary>
        /// <param name="personID">person id to search</param>
        /// <returns>returns matching person object</returns>
        Task<PersonResponse?> GetPersonByPersonoID(Guid? personID);

        /// <summary>
        /// used to return all persons objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns all the matching persons based on the given search field and serach string</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        /// <summary>
        /// it returns sorted list of persons
        /// </summary>
        /// <param name="allPersons">represents list of persons to be sorted</param>
        /// <param name="sortBy">name of the property (key), based on which persons should be sorted </param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>returns sorted persons as PersonResponse list</returns>
        
        Task<MemoryStream> GetPersonsCSV();

        /// <summary>
        /// Returns persons as Excel
        /// </summary>
        /// <returns>returns the memory stream with excel data of persons </returns>
        Task<MemoryStream> GetPersonsExcel();
    }
}
 