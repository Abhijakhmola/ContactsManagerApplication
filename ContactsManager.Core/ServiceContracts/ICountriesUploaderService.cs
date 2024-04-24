using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulation country entity
    /// </summary>
    public interface ICountriesUploaderService
    {
        

        /// <summary>
        /// Uploads countries from excel file into the database
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns>returns number of countries added</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}
