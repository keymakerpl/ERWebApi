using System;

namespace ERWebApi.Models
{
    /// <summary>
    /// Customer model.
    /// </summary>
    public class CustomerDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// First name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Company name.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// NIP.
        /// </summary>
        public string NIP { get; set; }

        /// <summary>
        /// Email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Secondary email.
        /// </summary>
        public string Email2 { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Secondary phone number.
        /// </summary>
        public string PhoneNumber2 { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; set; }
    }
}
