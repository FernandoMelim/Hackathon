
using HealthMed.Doctor.Domain.Entitites;

namespace HealthMed.Doctor.Domain.Contracts.Repositories;

public interface IDoctorRespository
{
    Task<int> AddAvailableAppointmentAsync(DoctorAvailableAppointment horarioDisponivel, CancellationToken cancellationToken);
    Task<DoctorEntity> GetByIdAsync(int doctorId, CancellationToken cancellationToken);
    Task<DoctorEntity> GetDoctorByCrm(string crm);
}
