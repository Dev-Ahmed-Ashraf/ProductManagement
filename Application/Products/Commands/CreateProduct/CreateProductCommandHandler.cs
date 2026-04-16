using AutoMapper;
using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Domain.Entities;
using DBS_Task.Infrastructure.Data.DBContext;
using MediatR;

namespace DBS_Task.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ApiResponse<ProductResponseDto>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<ApiResponse<ProductResponseDto>> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(request);

            await _dbContext.Products.AddAsync(product, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var createdProduct = _mapper.Map<ProductResponseDto>(product);

            return ApiResponse<ProductResponseDto>.SuccessResponse(
                createdProduct,
                201,
                "Product created successfully."
            );
        }
    }
}
