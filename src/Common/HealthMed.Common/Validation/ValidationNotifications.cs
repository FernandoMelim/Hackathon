namespace HealthMed.Common.Validation;

public class ValidationNotifications
{
    private IDictionary<string, List<string>> _errors;

    public ValidationNotifications()
    {
        _errors = new Dictionary<string, List<string>>();
    }

    public void AddError(string key, string error)
    {
        if (!_errors.ContainsKey(key))
            _errors.Add(key, new List<string>());

        _errors[key].Add(error);
    }

    public bool HasErrors()
        => _errors.Any();

    public IDictionary<string, List<string>> GetErrors()
        => _errors.ToDictionary(x => x.Key, y => y.Value);
}
