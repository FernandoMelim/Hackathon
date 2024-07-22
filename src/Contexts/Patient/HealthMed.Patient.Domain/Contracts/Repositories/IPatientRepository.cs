
using HealthMed.Patient.Domain.Entities;

namespace HealthMed.Patient.Domain.Contracts.Repositories;

public interface IPatientRepository
{
    Task<List<DoctorEntity>> GetDoctorByFilter(int? rating, int? doctorExpertiseId);
    Task<List<DoctorAvailableAppointment>> GetDoctorsAppointments(int id);
    Task<PatientEntity> GetPatientUsingCpfAndEmail(string cpf, string email);
    Task<PatientEntity> GetPatientUsingId(int id);
    Task InsertPatientAppointmentRequest(PatientAppointmentsEntity patientAppointment);
}
