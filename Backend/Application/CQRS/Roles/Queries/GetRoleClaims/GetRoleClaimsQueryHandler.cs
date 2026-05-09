using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Roles;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DBS_Task.Application.CQRS.Roles.Queries.GetRoleClaims
{
    public class GetRoleClaimsQueryHandler : IRequestHandler<GetRoleClaimsQuery, ApiResponse<List<RoleClaimResponse>>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetRoleClaimsQueryHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<ApiResponse<List<RoleClaimResponse>>> Handle(GetRoleClaimsQuery request, CancellationToken cancellationToken)
        {
           var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role is null)
            {
                return ApiResponse<List<RoleClaimResponse>>.FailureResponse("Role not found", 404);
            }
            var claims = await _roleManager.GetClaimsAsync(role);

            var response = claims.Select(c => new RoleClaimResponse
            {
                Type = c.Type,
                Value = c.Value
            }).ToList();

            return ApiResponse<List<RoleClaimResponse>>.SuccessResponse(response, 200, "Role claims retrieved successfully");
        }
    }
}
