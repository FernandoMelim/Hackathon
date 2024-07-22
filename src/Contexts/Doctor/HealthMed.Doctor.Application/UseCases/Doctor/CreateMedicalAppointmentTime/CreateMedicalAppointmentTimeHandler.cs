using HealthMed.Common.Exceptions;
using HealthMed.Doctor.Domain.Contracts.Repositories;
using HealthMed.Doctor.Domain.Entitites;
using MediatR;

namespace HealthMed.Doctor.Application.UseCases.Doctor.CreateMedicalAppointmentTime;

public class CreateMedicalAppointmentTimeHandler(IDoctorRespository doctorRepository) : IRequestHandler<CreateMedicalAppointmentTimeRequest, CreateMedicalAppointmentTimeResponse>
{
    public async Task<CreateMedicalAppointmentTimeResponse> Handle(CreateMedicalAppointmentTimeRequest request, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(request.doctorId, cancellationToken);
        if (doctor == null)
            throw new ObjectNotFoundException("Médico não encontrado na base de dados.");

        var availableAppointment = new DoctorAvailableAppointment
        {
            DoctorId = request.doctorId,
            StartDate = request.startDate,
            EndDate = request.endDate,
            Available = true
        };

        await doctorRepository.AddAvailableAppointmentAsync(availableAppointment, cancellationToken);

        return new CreateMedicalAppointmentTimeResponse();
    }
}
