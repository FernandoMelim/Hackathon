using HealthMed.Doctor.Domain.Contracts.Repositories;
using MediatR;

namespace HealthMed.Doctor.Application.UseCases.Doctor.AcceptAppointment;

public class AcceptAppointmentHandler(IDoctorRespository doctorRespository) : IRequestHandler<AcceptAppointmentRequest, AcceptAppointmentResponse>
{
    public async Task<AcceptAppointmentResponse> Handle(AcceptAppointmentRequest request, CancellationToken cancellationToken)
    {
        await doctorRespository.AcceptPendingMedicalAppointment(request.appointmentId, request.doctorAppointmentId);

        return new AcceptAppointmentResponse();
    }
}
