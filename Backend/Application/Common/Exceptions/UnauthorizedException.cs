namespace DBS_Task.Application.Common.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() : base("Authentication is required. Please login to continue.") { }
        public UnauthorizedException(string message)
            : base(message)
        {
        }
    }
}
