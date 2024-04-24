using Entities;
using ServiceContracts.Enums;
using System;


namespace ServiceContracts.DTO
{
    /// <summary>
    /// Returns DTO class that is used as a return type of most methods of Persons Service
    /// </summary>
    public class PersonResponse
    {
       public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }

        /// <summary>
        /// compares the current object data with the parameter object
        /// </summary>
        /// <param name="obj">the person object to compare </param>
        /// <returns>true or false, indicating whether all the persons details matched with the specified parameter object</returns>
        public override bool Equals(object? obj)
        {
            if(obj == null) return false;
            if(obj.GetType() != typeof(PersonResponse)) return false;
            PersonResponse person = (PersonResponse)obj;
            return this.PersonID == person.PersonID && this.PersonName == person.PersonName && this.Email == person.Email && this.DateOfBirth == person.DateOfBirth && this.Gender == person.Gender && this.CountryID == person.CountryID && this.Address == person.Address && this.ReceiveNewsLetters == person.ReceiveNewsLetters;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Person ID:{PersonID}, Person Name:{PersonName}, Email:${Email}, Date of Birth:{DateOfBirth?.ToString("dd MM yyyy")}, Gender:{Gender}, Country ID:{CountryID}, Country:{Country}, Address:{Address}, Receive News Letter:{ReceiveNewsLetters}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest() {
            return new PersonUpdateRequest() {
                personID = this.PersonID,
                PersonName = this.PersonName,
                Email = this.Email,
                Address = this.Address,
                DateOfBirth = this.DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions),this.Gender,true),
                CountryID = this.CountryID,
                ReceiveNewsLetters = this.ReceiveNewsLetters,

            };
        }
    }

    public static class PersonExtensions { 
        /// <summary>
        /// an extension method to convert an object of person class into PersonResponse class
        /// </summary>
        /// <param name="person"></param>
        /// <returns>Returns the converted PersonResponse object</returns>
        public static PersonResponse ToPersonResponse(this Person person){
            return new PersonResponse() { PersonID = person.PersonID, PersonName = person.PersonName,
                Email = person.Email ,DateOfBirth=person.DateOfBirth,ReceiveNewsLetters=person.ReceiveNewsLetters,
                Address=person.Address,CountryID=person.CountryID,Gender=person.Gender,
                Age=(person.DateOfBirth!=null)?
                Math.Round((DateTime.Now-person.DateOfBirth.Value).TotalDays/365.25):null,Country=person.Country?.CountryName};
        }


    }

    

}
