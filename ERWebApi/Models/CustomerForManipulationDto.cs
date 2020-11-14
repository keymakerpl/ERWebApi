using System.ComponentModel.DataAnnotations;

namespace ERWebApi.Models
{
    /// <summary>
    /// An customer for data manipulation.
    /// </summary>
    public abstract class CustomerForManipulationDto
    {
        /// <summary>
        /// First name of customer
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of customer
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        /// <summary>
        /// Company
        /// </summary>
        [MaxLength(100)]
        public string CompanyName { get; set; }

        /// <summary>
        /// NIP
        /// </summary>
        [MaxLength(50)]
        public string NIP { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [MaxLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// Secondary email
        /// </summary>
        [MaxLength(100)]
        public string Email2 { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        [MaxLength(100)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Secondary phone number
        /// </summary>
        [MaxLength(100)]
        public string PhoneNumber2 { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; }
    }
}
