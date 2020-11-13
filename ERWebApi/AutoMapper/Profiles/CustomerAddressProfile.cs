using AutoMapper;
using ERService.Business;
using ERWebApi.Models;

namespace ERWebApi.AutoMapper.Profiles
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CustomerAddressProfile : Profile
    {
        public CustomerAddressProfile()
        {
            CreateMap<CustomerAddress, CustomerAddressDto>();
            CreateMap<CustomerAddressDto, CustomerAddress>();
            CreateMap<CustomerAddressForCreateDto, CustomerAddress>();
            CreateMap<CustomerAddress, CustomerAddressForCreateDto>();
            CreateMap<CustomerAddressForUpdateDto, CustomerAddress>();
            CreateMap<CustomerAddress, CustomerAddressForUpdateDto>();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
