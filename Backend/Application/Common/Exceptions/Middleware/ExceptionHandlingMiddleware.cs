using DBS_Task.Application.Common.Exceptions;
using DBS_Task.Application.Common.Results;
using FluentValidation;
using System.Text.Json;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        ApiResponse<object> response;

        switch (exception)
        {
            case NotFoundException:

                context.Response.StatusCode = StatusCodes.Status404NotFound;

                response = ApiResponse<object>.FailureResponse(
                    message: exception.Message,
                    statusCode: StatusCodes.Status404NotFound);

                break;

            case UnauthorizedException:

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                response = ApiResponse<object>.FailureResponse(
                    message: exception.Message,
                    statusCode: StatusCodes.Status401Unauthorized);

                break;

            case ForbiddenException:

                context.Response.StatusCode = StatusCodes.Status403Forbidden;

                response = ApiResponse<object>.FailureResponse(
                    message: exception.Message,
                    statusCode: StatusCodes.Status403Forbidden);

                break;

            case BadRequestException:

                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                response = ApiResponse<object>.FailureResponse(
                    message: exception.Message,
                    statusCode: StatusCodes.Status400BadRequest);

                break;

            default:

                context.Response.StatusCode =
                    StatusCodes.Status500InternalServerError;

                response = ApiResponse<object>.FailureResponse(
                    message: "Internal server error",
                    statusCode: StatusCodes.Status500InternalServerError);

                break;
        }

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}