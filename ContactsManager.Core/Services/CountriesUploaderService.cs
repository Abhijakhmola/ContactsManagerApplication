﻿using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;

namespace Services
{
    public class CountriesUploaderService : ICountriesUploaderService
    {
        //private filed
        private readonly ICountriesRepository _countriesRepository;


        //constructor 
        public CountriesUploaderService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;

        }
       

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countriesInserted = 0;
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = worksheet.Dimension.Rows;


                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);

                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;
                        if (await _countriesRepository.GetCountryByCountryName(countryName)==null)
                        {
                            Country country = new Country()
                            {
                                CountryName = countryName,
                            };
                            //generate CountryID
                            // country.CountryId = Guid.NewGuid();
                           await  _countriesRepository.AddCountry(country);

                            countriesInserted++;
                        }
                    }
                }

            }

            return countriesInserted;

        }
    }
}
