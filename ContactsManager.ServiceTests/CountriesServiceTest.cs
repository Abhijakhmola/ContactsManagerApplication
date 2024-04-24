using ServiceContracts;
using System;
using Entities;
using Services;
using ServiceContracts.DTO;
using Moq;
using FluentAssertions;
using RepositoryContracts;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesAdderService _countriesAdderService;
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ICountriesUploaderService _countriesUploaderService;
        private readonly Mock<ICountriesRepository> _countryRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;

        //constructor
        public CountriesServiceTest()
        {
            _countryRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countryRepositoryMock.Object;
            _countriesAdderService = new CountriesAdderService(_countriesRepository);
            _countriesGetterService = new CountriesGetterService(_countriesRepository);
            _countriesUploaderService = new CountriesUploaderService(_countriesRepository);
        }

        #region AddCountry
        //when CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            //Arrage
            CountryAddRequest? request = null;


            Func<Task> action = async () =>
            {
                //Act
                await _countriesAdderService.AddCountry(request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //when the CountryName is null, it should throw ArgumentException

        [Fact]
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
        {
            //Arrage
            CountryAddRequest? request = new CountryAddRequest() { CountryName = null };


            Func<Task> action = async () =>
            {
                //Act
                await _countriesAdderService.AddCountry(request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //when the CountryName is duplicate, it should throw ArgumentException

        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
        {
            //Arrage
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "USA" };

            _countryRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(request1.ToCountry());

            Func<Task> action = async () =>
             {
                 //Act
                 await _countriesAdderService.AddCountry(request1);
                 await _countriesAdderService.AddCountry(request2);
             };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //when you supply proper CountryName, it should insert (add) the country to the existing list of the countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrage
            CountryAddRequest? request = new CountryAddRequest() { CountryName = "Japan" };
            _countryRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(request.ToCountry());
            _countryRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(new List<Country>() { request.ToCountry() });
            //Act
            CountryResponse response = await _countriesAdderService.AddCountry(request);
            response.CountryID = Guid.NewGuid();
            List<CountryResponse> countries_from_GetAllcountries = await _countriesGetterService.GetAllCountries();
            countries_from_GetAllcountries[0].CountryID = response.CountryID;

            //Assert
            // Assert.True(response.CountryID != Guid.Empty);
            response.CountryID.Should().NotBe(Guid.Empty);
            // Assert.Contains(response,countries_from_GetAllcountries);
            countries_from_GetAllcountries.Should().Contain(response);

        }
        #endregion

        #region GetAllCountries

        [Fact]
        //the list of countries should be empty by default(before adding any countries)
        public async Task GetAllCountries_EmptyList()
        {
            //Arrange
            _countryRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(new List<Country>() { });

            //Act
            List<CountryResponse> actual_country_response_list = await _countriesGetterService.GetAllCountries();

            //Assert
            // Assert.Empty(actual_country_response_list);
            actual_country_response_list.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            
            //Arrange
            List<Country> country_list = new List<Country>() {new Country() { CountryId=Guid.NewGuid(),CountryName="Japan"},
                new Country() { CountryId=Guid.NewGuid(),CountryName="China"},
      };

            List<CountryResponse> country_response_list = country_list.Select(temp => temp.ToCountryResponse()).ToList();

            _countryRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_list);

            //Act
            List<CountryResponse> actualCountryResponseList = await _countriesGetterService.GetAllCountries();

            //Assert
            actualCountryResponseList.Should().BeEquivalentTo(country_response_list);
        }
        #endregion

        #region GetCountryByCountryId

        [Fact]
        //if we supply null as countryId, it should return the null as countryResponse
        public async Task GetCountryByCountryId_NullCountryId()
        {
            //Arrange
            Guid? countryId = null;

            //Act
            CountryResponse? country_response_from_get_method = await _countriesGetterService.GetCountryByCountryId(countryId);

            //Assert
            //  Assert.Null(country_response_from_get_method);
            country_response_from_get_method.Should().BeNull();
        }

        [Fact]
        //if we supply a valid countryId , it should return the matching country details as countryResponse object
        public async Task GetCountryByCountryId_ValidCountry()
        {
          




            //Arrange
            Country country = new Country() { CountryId=Guid.NewGuid(),CountryName="India"};
            CountryResponse country_response = country.ToCountryResponse();

            _countryRepositoryMock
             .Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
             .ReturnsAsync(country);

            //Act
            CountryResponse? country_response_from_get = await _countriesGetterService.GetCountryByCountryId(country.CountryId);


            //Assert
            country_response_from_get.Should().Be(country_response);
        }
        #endregion

    }
}
