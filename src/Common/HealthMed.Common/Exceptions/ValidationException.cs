using HealthMed.Common.Validation;

namespace HealthMed.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationNotifications ValidationNotifications { get; private set; }

    public ValidationException(ValidationNotifications validationNotifications)
    {
        ValidationNotifications = validationNotifications ?? throw new ArgumentNullException(nameof(validationNotifications));
    }
}
