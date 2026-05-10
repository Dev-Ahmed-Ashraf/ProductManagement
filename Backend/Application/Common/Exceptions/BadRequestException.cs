namespace DBS_Task.Application.Common.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException() : base("The request is invalid.") { }

        public BadRequestException(string message)
            : base(message)
        {
        }
    }
}
