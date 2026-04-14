using System.Net;

namespace DBS_Task.API.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; private set; }
        public string? Message { get; private set; }
        public T? Data { get; private set; }
        public List<string>? Errors { get; private set; }
        public HttpStatusCode StatusCode { get; set; }


        public static ApiResponse<T> SuccessResponse(T? data, int statusCode = 200, string? message = null) =>
        new() { Success = true, Data = data, Message = message, StatusCode = (HttpStatusCode)statusCode };

        public static ApiResponse<T> FailureResponse(string message, int statusCode = 400, List<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors, StatusCode = (HttpStatusCode)statusCode };
    }
}
