using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Person enitity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Adds  a person object to the data store
        /// </summary>
        /// <param name="person">Person object to add</param>
        /// <returns>returns the person object after adding it to the table</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Returns all persons in the data store
        /// </summary>
        /// <returns>List of person objects from the table</returns>
        Task<List<Person>> GetAllPersons();
        /// <summary>
        /// returns a person object based on the personID
        /// </summary>
        /// <param name="personID">personId(Guid) to search</param>
        /// <returns>a person object or null</returns>
        Task<Person?> GetPersonByPersonID(Guid personID);
        /// <summary>
        /// returns all persons object based on the given expression
        /// </summary>
        /// <param name="predicate">Linq expression to check</param>
        /// <returns>all matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);
        /// <summary>
        /// Deletes a person object based on the person id 
        /// </summary>
        /// <returns>returns true if deleted successfully else false</returns>
        Task<bool> DeletePersonByPersonID(Guid personID);
        /// <summary>
        /// Updates a person object(person name and other details ) based on the given person id
        /// </summary>
        /// <param name="person">person object to update</param>
        /// <returns>returns the updated person object</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
