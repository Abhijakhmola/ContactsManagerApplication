using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;
using Serilog;
using Microsoft.Extensions.Logging;


namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //private fields
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        //constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture=new Fixture();
            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository=_personRepositoryMock.Object;

            var diagnosticContextMock = new Mock<IDiagnosticContext>();
            var loggerMockA=new Mock<ILogger<PersonsAdderService>>();
            var loggerMockG=new Mock<ILogger<PersonsGetterService>>();
            var loggerMockD=new Mock<ILogger<PersonsDeleterService>>();
            var loggerMockU=new Mock<ILogger<PersonsUpdaterService>>();
            var loggerMockS=new Mock<ILogger<PersonsSorterService>>();

            _personsGetterService = new PersonsGetterService(_personsRepository, loggerMockG.Object, diagnosticContextMock.Object);
            _personsAdderService = new PersonsAdderService(_personsRepository, loggerMockA.Object, diagnosticContextMock.Object);
            _personsDeleterService = new PersonsDeleterService(_personsRepository, loggerMockD.Object, diagnosticContextMock.Object);
            _personsUpdaterService = new PersonsUpdaterService(_personsRepository, loggerMockU.Object, diagnosticContextMock.Object);
            _personsSorterService = new PersonsSorterService(loggerMockS.Object, diagnosticContextMock.Object);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        //when we supply null as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Func<Task> action = async () => {
                await _personsAdderService.AddPerson(personAddRequest);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
            //await Assert.ThrowsAsync<ArgumentNullException>

        }

        //when we supply null value as a PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp=>temp.PersonName,null as string).Create();

           
           Func<Task> action=async () =>
            {
                //Act
               await  _personsAdderService.AddPerson(personAddRequest);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();

        }

        //when we supply proper person details , it should insert the person into the persons list, and it should an object
        //of PersonResponse, which includes with the newly generated person id
        [Fact]
        public async Task  AddPerson_FullPersonDetails_ToBeSuccessfull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"someone@gmail.com").Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();

            //if we supply any argument valuet to AddPerson method,it should return the same return value
            _personRepositoryMock.Setup(temp=>temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act 
            PersonResponse person_resposne_from_add =await _personsAdderService.AddPerson(personAddRequest);
            person_response_expected.PersonID=person_resposne_from_add.PersonID;

            //Assert
            // Assert.True(person_resposne_from_add.PersonID != Guid.Empty);
            person_resposne_from_add.PersonID.Should().NotBe(Guid.Empty);
            person_resposne_from_add.Should().Be(person_response_expected);

        }
        #endregion

        #region GetPersonByPersonID
        //if we supply null as person id , it should return null response
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
        {
            //Arrange
            Guid? personID = null;

            //act
            PersonResponse? person_response_from_get =await _personsGetterService.GetPersonByPersonoID(personID);

            //assert
            // Assert.Null(person_response_from_get);
            person_response_from_get.Should().BeNull();
        }

        //if we supply a valid person id , it should return the valid person details as PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID_ToBeSucessfull()
        {
            //Arrange
            Person person = _fixture.Build<Person>().With(temp => temp.Email, "abc@gmail.com").With(temp=>temp.Country,null as Country).Create();
            PersonResponse person_response_expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            PersonResponse? person_response_from_get =await _personsGetterService.GetPersonByPersonoID(person.PersonID);

            //Assert    
           // Assert.Equal(person_response_from_add, person_response_from_get);
           person_response_from_get.Should().Be(person_response_expected);
        }
        #endregion

        #region GetAllPersons
        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPerson_ToEmptyList()
        {
            //Arrange
            _personRepositoryMock.Setup(temp=>temp.GetAllPersons()).ReturnsAsync(new List<Person>());

            //Act
            List<PersonResponse> persons_from_get =await _personsGetterService.GetAllPersons();

            //Assert
            //Assert.Empty(persons_from_get);
            persons_from_get.Should().BeEmpty();
        }

        //First, we will add few persons and then when we call GetAllPersons() , it should return the same persons that were added 
        [Fact]
        public async Task GetAllPerson_WithFewPersons_ToBeSucessfull()
        {
            //Arrange
            List<Person> persons = new List<Person>() { _fixture.Build<Person>().With(temp => temp.Email, "someone@gmail.com").With(temp=>temp.Country,null as Country).Create() , _fixture.Build<Person>().With(temp => temp.Email, "some@gmail.com").With(temp => temp.Country, null as Country).Create() };


           

            List<PersonResponse> person_response_list_expected = persons.Select(temp=>temp.ToPersonResponse()).ToList();

            

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_expected in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_expected.ToString());
            }

            _personRepositoryMock.Setup(temp=>temp.GetAllPersons()).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_response_list_from_get =await _personsGetterService.GetAllPersons();
            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_response_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //    Assert.Contains(person_response_from_add, persons_response_list_from_get);
            //}

            persons_response_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion

        #region GetFilteredPersons
        //If the search text is empty and search by is "PersonName" , it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessfull()
        {
            //Arrange
            List<Person> persons = new List<Person>() { _fixture.Build<Person>().With(temp => temp.Email, "someone_1@gmail.com").With(temp => temp.Country, null as Country).Create() , _fixture.Build<Person>().With(temp => temp.Email, "someone_2@gmail.com").With(temp => temp.Country, null as Country).Create() };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:"); 
            foreach (PersonResponse person_response_expected in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_expected.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_response_list_from_search =await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "");
            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_response_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_response_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

        //search based on person name with some search string. It should return the matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>() { _fixture.Build<Person>().With(temp => temp.Email, "someone_1@gmail.com").With(temp => temp.Country, null as Country).Create(), _fixture.Build<Person>().With(temp => temp.Email, "someone_2@gmail.com").With(temp => temp.Country, null as Country).Create() };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_expected in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_expected.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_response_list_from_search = await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "sa");
            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual:");   
            foreach (PersonResponse person_response_from_get in persons_response_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_response_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion

        #region GetSortedPersons
        //When we sort based on the person name in DESC , it should return persons list in descending on PersonName
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>() { _fixture.Build<Person>().With(temp => temp.Email, "someone_1@gmail.com").With(temp => temp.Country, null as Country).Create(), _fixture.Build<Person>().With(temp => temp.Email, "someone_2@gmail.com").With(temp => temp.Country, null as Country).Create() };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            
            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_expected in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_expected.ToString()); 
            }
            List<PersonResponse> allPersons=await _personsGetterService.GetAllPersons();
            //Act
            List<PersonResponse> persons_response_list_from_sort =await _personsSorterService.GetSortedPersons(allPersons,nameof(Person.PersonName), SortOrderOptions.DESC);
            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_response_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
           
            //Assert
            persons_response_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
        }

        #endregion

        #region UpdatePerson
        //if we supply null as a personupdaterequest , it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException() { 
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;
            
           Func<Task> action= async() => {
                //Act
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //if the person id is invalid , it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = _fixture.Create<PersonUpdateRequest>();

            
            Func<Task> action=async() => {
                //Act
               await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //when the personName is null it should throw argumentexception
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            
            Person person =_fixture.Build<Person>().With(temp=>temp.Email,"abc@gmail.com").With(temp=>temp.PersonName,null as string).With(temp=>temp.Country,null as Country).With(temp=>temp.Gender,"Male").Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            PersonUpdateRequest? personUpdateRequest = person_response_from_add.ToPersonUpdateRequest();

            
            Func<Task> action=async() => {
                //Act
               await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //First we will add a new peron and then try to update person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetialsUpdation_ToBeSuccessFull()
        {
            //Arrange
            Person person = _fixture.Build<Person>().With(temp => temp.Email, "abc@gmail.com").With(temp => temp.Gender, "Male").With(temp => temp.Country, null as Country).Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest? personUpdateRequest = person_response_expected.ToPersonUpdateRequest();

            _personRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update=await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            
            //Assert
            //Assert.Equal(person_response_from_get,person_response_from_update);
            person_response_from_update.Should().Be(person_response_expected);
        }
        #endregion

        #region DeletePerson

        //if we supply an valid person id , it should return true;
        [Fact]
        public async Task DeletePerson_ValidPersonID_ToBeSuccessful(){
            //Arrange
            Person person = _fixture.Build<Person>().With(temp=>temp.Email,"abc@gmail.com").With(temp=>temp.Country,null as Country).With(temp=>temp.Gender,"Male").Create();


            _personRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true);
            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            bool isDeleted=await _personsDeleterService.DeletePerson(person.PersonID);

            //Assert
            //Assert.True(isDeleted);
            isDeleted.Should().BeTrue();
        }

        //if we supply an invalid person id , it should return false;
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Arragne
            _personRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(false);

            //Act
            bool isDeleted =await _personsDeleterService.DeletePerson(Guid.NewGuid());

            //Assert
            // Assert.False(isDeleted);
            isDeleted.Should().BeFalse();
        }
        #endregion
    }
}
