using MediatR;

namespace HealthMed.Doctor.Application.UseCases.Doctor.CreateMedicalAppointmentTime;

public record CreateMedicalAppointmentTimeRequest(DateTime startDate, DateTime endDate, int doctorId) : IRequest<CreateMedicalAppointmentTimeResponse>;