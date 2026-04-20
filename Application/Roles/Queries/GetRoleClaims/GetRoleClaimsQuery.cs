using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Roles;
using MediatR;

namespace DBS_Task.Application.Roles.Queries.GetRoleClaims
{
    public record GetRoleClaimsQuery(string RoleId) : IRequest<ApiResponse<List<RoleClaimResponse>>>
    {
    }
}
