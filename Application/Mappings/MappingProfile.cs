using AutoMapper;
using DBS_Task.Application.Contracts;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Domain.Entities;

namespace DBS_Task.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateProductContract, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            CreateMap<Product, ProductResponseDto>();
        }
    }
}
