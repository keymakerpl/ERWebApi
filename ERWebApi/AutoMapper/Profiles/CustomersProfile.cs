using AutoMapper;
using ERService.Business;
using ERWebApi.Models;

namespace ERWebApi.AutoMapper.Profiles
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CustomersProfile : Profile

    {
        public CustomersProfile()
        {
            CreateMap<Customer, CustomerDto>()
                .ForMember(
                    dest => dest.FullName, 
                    from => from.MapFrom(src => $"{src.FirstName ?? ""} {src.LastName ?? ""}")
                    );

            CreateMap<CustomerForUpdateDto, Customer>();
            CreateMap<CustomerForCreationDto, Customer>();
            CreateMap<Customer, CustomerForCreationDto>();
            CreateMap<Customer, CustomerForUpdateDto>();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
