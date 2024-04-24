using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly ILogger<PersonsRepository> _logger;
        public PersonsRepository(ApplicationDbContext dbcontext,ILogger<PersonsRepository> logger) {
            _dbcontext = dbcontext;
            _logger = logger;
        }
        public async Task<Person> AddPerson(Person person)
        {
            _dbcontext.Persons.Add(person);
            await _dbcontext.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByPersonID(Guid personID)
        { 
            _dbcontext.Persons.RemoveRange(_dbcontext.Persons.Where(temp => temp.PersonID == personID));
            int rowsDeleted=await _dbcontext.SaveChangesAsync();
            return rowsDeleted>0 ;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsRepository");


            return await _dbcontext.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsRepository");

            return await _dbcontext.Persons.Include("Country").Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonID(Guid personID)
        {
            return await _dbcontext.Persons.Include("Country").FirstOrDefaultAsync(temp=>temp.PersonID==personID);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingperson = await _dbcontext.Persons.FirstOrDefaultAsync(temp => temp.PersonID == person.PersonID);
            if(matchingperson==null) { return person; }

            matchingperson.PersonName = person.PersonName;
            matchingperson.Email = person.Email;
            matchingperson.DateOfBirth = person.DateOfBirth;
            matchingperson.Gender = person.Gender;
            matchingperson.Address = person.Address;
            matchingperson.CountryID = person.CountryID;
            matchingperson.ReceiveNewsLetters = person.ReceiveNewsLetters;

            await _dbcontext.SaveChangesAsync();
            return matchingperson;
        }
    }
}
