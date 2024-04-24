using Entities;
using Exceptions;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services
{
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        //private field
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsUpdaterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;


        //constructor
        public PersonsUpdaterService(IPersonsRepository personsRepository, ILogger<PersonsUpdaterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }


      

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null) throw new ArgumentNullException(nameof(personUpdateRequest));

            //validation 
            ValidationHelper.ModelValidation(personUpdateRequest);

            //get matching person object
            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personUpdateRequest.personID);

            if (matchingPerson == null)
            {
                throw new InvalidPersonIDException("Given person id doesn't exist.");
            }

            //update all details 
            await _personsRepository.UpdatePerson(personUpdateRequest.ToPerson());
            return matchingPerson.ToPersonResponse();
        }

     
    }
}
