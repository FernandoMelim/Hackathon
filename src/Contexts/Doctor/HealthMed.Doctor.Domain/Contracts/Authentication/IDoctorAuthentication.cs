
using HealthMed.Doctor.Domain.Entitites;

namespace HealthMed.Doctor.Domain.Contracts.Authentication;

public interface IDoctorAuthentication
{
    Task<string> AuthenticateDoctor(string doctorEmail, string doctorPassword);
}
