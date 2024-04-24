using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person Entity
    /// </summary>
    public  interface IPersonsDeleterService
    {
        /// <summary>
        /// Deletes a person based on the given person id 
        /// </summary>
        /// <param name="PersonID">Person ID to delete</param>
        /// <returns>Returns true if deletion is successful otherwise false</returns>
        Task<bool> DeletePerson(Guid? PersonID);

    }
}
 