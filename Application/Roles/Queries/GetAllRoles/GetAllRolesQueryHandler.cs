using AutoMapper;
using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Roles;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Application.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, ApiResponse<List<RoleResponse>>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public GetAllRolesQueryHandler(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }
        public async Task<ApiResponse<List<RoleResponse>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles.ToListAsync(cancellationToken);
            if (roles == null || roles.Count == 0)
            {
                return ApiResponse<List<RoleResponse>>.FailureResponse("No roles found", 404);
            }

            var roleResponses = _mapper.Map<List<RoleResponse>>(roles);

            return ApiResponse<List<RoleResponse>>.SuccessResponse(roleResponses, 200, "Roles retrieved successfully");
        }
    }
}
