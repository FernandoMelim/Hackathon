using MediatR;

namespace HealthMed.Patient.Application.UseCases.Patient.ScheduleAppointment
{
    public record ScheduleAppointmentRequest(int patientId, int appointmentId) : IRequest<ScheduleAppointmentResponse>;
}
