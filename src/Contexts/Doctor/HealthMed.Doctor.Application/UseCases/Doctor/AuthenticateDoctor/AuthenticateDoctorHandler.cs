using HealthMed.Common.Exceptions;
using HealthMed.Doctor.Domain.Contracts.Authentication;
using HealthMed.Doctor.Domain.Contracts.Repositories;
using MediatR;

namespace HealthMed.Doctor.Application.UseCases.Doctor.AuthenticateDoctor;

public class AuthenticateDoctorHandler(IDoctorRespository doctorRepository, IDoctorAuthentication doctorAuthentication) : IRequestHandler<AuthenticateDoctorRequest, AuthenticateDoctorResponse>
{
    public async Task<AuthenticateDoctorResponse> Handle(AuthenticateDoctorRequest request, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetDoctorByCrm(request.Crm)
        ?? throw new ObjectNotFoundException("Não há nenhum médico cadastrado com esse CRM.");

        var accessToken = await doctorAuthentication.AuthenticateDoctor(doctor.Email, request.Password);

        return new AuthenticateDoctorResponse(accessToken);
    }
}
