using System;

namespace ERWebApi.Models
{
    public class OrderDto
    {
        public Guid Id { get; set; }

        public int OrderId { get; set; }

        public string Number { get; set; }

        public string OrderNumber { get; set; }

        public DateTime DateRegistered { get; set; }

        public DateTime? DateEnded { get; set; }

        public OrderStatusDto OrderStatus { get; set; }

        public OrderTypeDto OrderType { get; set; }

        public UserDto User { get; set; }

        public string Cost { get; set; }

        public string Fault { get; set; }

        public string Solution { get; set; }

        public string Comment { get; set; }

        public string ExternalNumber { get; set; }

        public int Progress { get; set; }
    }
}
