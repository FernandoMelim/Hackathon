using MediatR;

namespace HealthMed.Doctor.Application.UseCases.Doctor.GetPendingMedicalAppointments;

public record GetPendingMedicalAppointmentsRequest(int doctorId) : IRequest<GetPendingMedicalAppointmentsResponse>
{
}
