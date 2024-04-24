using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person Entity
    /// </summary>
    public  interface IPersonsUpdaterService
    {
       
        /// <summary>
        /// Updates the specified person details based on the given person id
        /// </summary>
        /// <param name="personUpdateRequest">person details to update</param>
        /// <returns>Returns the person object after updation</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

    }
}
 