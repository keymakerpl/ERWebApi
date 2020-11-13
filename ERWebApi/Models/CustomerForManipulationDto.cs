using System.ComponentModel.DataAnnotations;

namespace ERWebApi.Models
{
    /// <summary>
    /// An customer for data manipulation.
    /// </summary>
    public abstract class CustomerForManipulationDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string CompanyName { get; set; }

        [MaxLength(50)]
        public string NIP { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Email2 { get; set; }

        [MaxLength(100)]
        public string PhoneNumber { get; set; }

        [MaxLength(100)]
        public string PhoneNumber2 { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }
    }
}
