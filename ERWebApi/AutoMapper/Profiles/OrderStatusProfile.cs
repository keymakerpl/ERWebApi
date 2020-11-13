using AutoMapper;
using ERService.Business;
using ERWebApi.Models;

namespace ERWebApi.AutoMapper.Profiles
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class OrderStatusProfile : Profile
    {
        public OrderStatusProfile()
        {
            CreateMap<OrderStatus, OrderStatusDto>();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
