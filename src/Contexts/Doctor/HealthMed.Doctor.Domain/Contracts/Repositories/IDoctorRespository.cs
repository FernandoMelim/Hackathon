
using HealthMed.Doctor.Domain.Entitites;
using HealthMed.Doctor.Domain.Models;

namespace HealthMed.Doctor.Domain.Contracts.Repositories;

public interface IDoctorRespository
{
    Task AcceptPendingMedicalAppointment(int appointmentId, int doctorAppointmentId);
    Task<int> AddAvailableAppointmentAsync(DoctorAvailableAppointment horarioDisponivel, CancellationToken cancellationToken);
    Task<DoctorEntity> GetByIdAsync(int doctorId, CancellationToken cancellationToken);
    Task<DoctorEntity> GetDoctorByCrm(string crm);
    Task<List<PatientsPendingAppointments>> GetPendingAppointments(int doctorId);
}
