using HealthMed.Doctor.Domain.Models;

namespace HealthMed.Doctor.Application.UseCases.Doctor.GetPendingMedicalAppointments
{
    public record GetPendingMedicalAppointmentsResponse
    {
        public List<PatientsPendingAppointments> pendingAppointments { get; set; } = new List<PatientsPendingAppointments>();
    }
}