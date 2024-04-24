using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Country
    {
        /// <summary>
        /// this is the domain model for storing the country details
        /// </summary>
        [Key]
        public Guid CountryId { get; set; }
        public string? CountryName {  get; set; }

        public virtual ICollection<Person>? Persons{ get;set;}

    }
}
