using HealthMed.Doctor.Domain.Contracts.Repositories;
using MediatR;

namespace HealthMed.Doctor.Application.UseCases.Doctor.GetPendingMedicalAppointments;

public class GetPendingMedicalAppointmentsHandler(IDoctorRespository doctorRespository) : IRequestHandler<GetPendingMedicalAppointmentsRequest, GetPendingMedicalAppointmentsResponse>
{
    public async Task<GetPendingMedicalAppointmentsResponse> Handle(GetPendingMedicalAppointmentsRequest request, CancellationToken cancellationToken)
    {
        var pendingAppointments = await doctorRespository.GetPendingAppointments(request.doctorId);

        return new GetPendingMedicalAppointmentsResponse() { pendingAppointments = pendingAppointments };
    }
}
