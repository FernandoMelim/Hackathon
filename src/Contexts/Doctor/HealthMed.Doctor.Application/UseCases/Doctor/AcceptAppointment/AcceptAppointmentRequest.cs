using MediatR;

namespace HealthMed.Doctor.Application.UseCases.Doctor.AcceptAppointment;

public record AcceptAppointmentRequest(int appointmentId, int doctorAppointmentId) : IRequest<AcceptAppointmentResponse>;
