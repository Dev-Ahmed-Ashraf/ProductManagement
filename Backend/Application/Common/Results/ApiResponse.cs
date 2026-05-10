using System.Net;

namespace DBS_Task.Application.Common.Results
{
    public class ApiResponse<T>
    {
        public bool success { get; private set; }

        public string? message { get; private set; }

        public T? data { get; private set; }

        public List<string>? errors { get; private set; }

        public HttpStatusCode statusCode { get; private set; }

        public static ApiResponse<T> SuccessResponse(
            T? data,
            int statusCode = 200,
            string? message = null)
        {
            return new ApiResponse<T>
            {
                success = true,
                data = data,
                message = message,
                statusCode = (HttpStatusCode)statusCode
            };
        }

        public static ApiResponse<T> FailureResponse(
            string message,
            int statusCode = 400,
            List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                success = false,
                message = message,
                errors = errors,
                statusCode = (HttpStatusCode)statusCode
            };
        }
    }
}
