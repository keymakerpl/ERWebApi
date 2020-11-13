using AutoMapper;
using ERService.Business;
using ERWebApi.Models;

namespace ERWebApi.AutoMapper.Profiles
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class OrderTypeProfile : Profile
    {
        public OrderTypeProfile()
        {
            CreateMap<OrderType, OrderTypeDto>();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
