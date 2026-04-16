using DBS_Task.Application.Common.Results;
using FluentValidation;
using MediatR;

namespace DBS_Task.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                var errors = failures
                    .Select(f => f.ErrorMessage)
                    .Distinct()
                    .ToList();

                var responseType = typeof(TResponse);

                if (responseType.IsGenericType &&
                    responseType.GetGenericTypeDefinition() == typeof(ApiResponse<>))
                {
                    var dataType = responseType.GetGenericArguments()[0];

                    var failureMethod = typeof(ApiResponse<>)
                        .MakeGenericType(dataType)
                        .GetMethod(nameof(ApiResponse<object>.FailureResponse));

                    var result = failureMethod!.Invoke(null, new object[]
                    {
                    "Validation Failed",
                    400,
                    errors
                    });

                    return (TResponse)result!;
                }

                throw new ValidationException(failures);
            }

            return await next();
        }
    }
}
