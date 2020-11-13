using System.ComponentModel.DataAnnotations;

namespace ERWebApi.Models
{
    public class CustomerAddressForManipulationDto
    {
        [MaxLength(100)]
        public string Street { get; set; }

        [MaxLength(100)]
        public string HouseNumber { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string Postcode { get; set; }
    }
}
