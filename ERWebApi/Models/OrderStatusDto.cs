using ERService.Business;
using System;

namespace ERWebApi.Models
{
    public class OrderStatusDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public StatusGroup Group { get; set; }
    }
}
