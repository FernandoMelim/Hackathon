
using HealthMed.Patient.Domain.Entities;

namespace HealthMed.Patient.Domain.Contracts.Repositories;

public interface IPatientRepository
{
    Task<PatientEntity> GetPatientUsingCpfAndEmail(string cpf, string email);
}
