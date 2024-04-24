using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulation country entity
    /// </summary>
    public interface ICountriesAdderService
    {
        /// <summary>
        /// adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to be added</param>
        /// <returns>returns the country object after adding it (including newly generated country id )</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        
    }
}
