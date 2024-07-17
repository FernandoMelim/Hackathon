using FluentValidation;
using MediatR;
using ValidationException = HealthMed.Common.Exceptions.ValidationException;

namespace HealthMed.Common.Validation;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest> validator, ValidationNotifications validationNotifications) : IPipelineBehavior<TRequest, TResponse>
                                              where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse>
                                                next, CancellationToken cancellationToken)
    {
        if (validator is null)
            return await next();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors;

            foreach (var error in errors)
            {
                validationNotifications.AddError(error.PropertyName, error.ErrorMessage);
            }

            throw new ValidationException(validationNotifications);
        }


        return await next();
    }
}
