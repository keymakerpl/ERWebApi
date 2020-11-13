using System;

namespace ERWebApi.Models
{
    public class CustomerAddressDto
    {
        public Guid Id { get; set; }

        public string Street { get; set; }

        public string HouseNumber { get; set; }

        public string City { get; set; }

        public string Postcode { get; set; }
    }
}
