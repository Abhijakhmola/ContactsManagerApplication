using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly ApplicationDbContext _dbcontext;

        public CountriesRepository(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<Country> AddCountry(Country country)
        {
            _dbcontext.Countries.Add(country);
            await _dbcontext.SaveChangesAsync();

            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
           return await _dbcontext.Countries.ToListAsync();

        }

        public async Task<Country?> GetCountryByCountryID(Guid countryID)
        {
            return await _dbcontext.Countries.FirstOrDefaultAsync(temp => temp.CountryId == countryID);
        }

        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
            return await _dbcontext.Countries.FirstOrDefaultAsync(temp=>temp.CountryName.Equals(countryName));
        }
    }
}
