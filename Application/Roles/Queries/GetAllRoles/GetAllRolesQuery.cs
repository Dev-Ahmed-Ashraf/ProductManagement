using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Roles;
using MediatR;

namespace DBS_Task.Application.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQuery : IRequest<ApiResponse<List<RoleResponse>>>
    {
    }
}
