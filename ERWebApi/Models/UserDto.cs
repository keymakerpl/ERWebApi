using ERService.Business;
using System;

namespace ERWebApi.Models
{

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class UserDto
    {
        public Guid Id { get; set; }

        public string Login { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public Guid? RoleId { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
