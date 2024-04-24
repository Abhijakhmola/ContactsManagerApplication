using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulation country entity
    /// </summary>
    public interface ICountriesGetterService
    {
        

        /// <summary>
        /// returns all countries from the list
        /// </summary>
        /// <returns>all the countries from the list as  list of countryResponse</returns>
        Task<List<CountryResponse>> GetAllCountries();
        /// <summary>
        /// returns a country object based on the countryId
        /// </summary>
        /// <param name="countryId">Country id(guid) which you want to search </param>
        /// <returns>matching country as countryResponse object</returns>
        Task<CountryResponse?> GetCountryByCountryId(Guid? countryId);

    }
}
