
namespace HealthMed.Patient.Domain.Contracts.Authentication;

public interface IPatientAuthentication
{
    Task<string> AuthenticatePatient(string email, string password);
}
