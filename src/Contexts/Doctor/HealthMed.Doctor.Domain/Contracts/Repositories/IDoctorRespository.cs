
using HealthMed.Doctor.Domain.Entitites;

namespace HealthMed.Doctor.Domain.Contracts.Repositories;

public interface IDoctorRespository
{
    Task<DoctorEntity> GetDoctorByCrm(string crm);
}
