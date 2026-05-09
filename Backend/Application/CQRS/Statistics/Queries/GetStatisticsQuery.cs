using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Statistics;
using MediatR;

namespace DBS_Task.Application.CQRS.Statistics.Queries
{
    public record GetStatisticsQuery : IRequest<ApiResponse<StatisticsResponseDto>>
    {
    }
}
