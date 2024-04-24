using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Country entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// adds a new country object to the data store
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>returns the country object after adding it to the data store</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// returns all countries in the datastore
        /// </summary>
        /// <returns>all countries from the table</returns>
        Task<List<Country>> GetAllCountries();
        /// <summary>
        /// returns a country object based on the given country id , otherwise it returns null
        /// </summary>
        /// <param name="countryID">country id to search</param>
        /// <returns>matching country or null</returns>
        Task<Country?> GetCountryByCountryID(Guid countryID);
        /// <summary>
        /// returns a country object based on the given country name otherwise it returns null
        /// </summary>
        /// <param name="countryName">country name to search</param>
        /// <returns>matching country or null</returns>
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}
