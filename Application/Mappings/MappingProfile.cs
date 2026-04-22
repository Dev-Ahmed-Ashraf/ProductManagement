using AutoMapper;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Application.DTOs.ProductStatusHistory;
using DBS_Task.Application.DTOs.Roles;
using DBS_Task.Application.Products.Commands.CreateProduct;
using DBS_Task.Contracts;
using DBS_Task.Domain.Entities;
using Microsoft.AspNetCore.Identity;

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

            // Added mapping logic for ProductStatusHistory -> DTO
            CreateMap<ProductStatusHistory, ProductStatusHistoriesResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            // Added mapping logic for IdentityRole -> RoleResponse
            CreateMap<IdentityRole, RoleResponse>();
        }
    }
}
