namespace DBS_Task.API.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; private set; }
        public string? Message { get; private set; }
        public T? Data { get; private set; }
        public List<string>? Errors { get; private set; }

        public static ApiResponse<T> SuccessResponse(T? data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> FailureResponse(string message, List<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };
    }
}
