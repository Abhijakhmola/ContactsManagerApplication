﻿using Entities;

using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesGetterService : ICountriesGetterService
    {
        //private filed
        private readonly ICountriesRepository _countriesRepository;


        //constructor 
        public CountriesGetterService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;

        }
       
        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries()).Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
        {
            if (countryId == null) { return null; }

            Country? country_response_from_list = await _countriesRepository.GetCountryByCountryID(countryId.Value);


            if (country_response_from_list == null) return null;

            return country_response_from_list.ToCountryResponse();
        }

       
    }
}
