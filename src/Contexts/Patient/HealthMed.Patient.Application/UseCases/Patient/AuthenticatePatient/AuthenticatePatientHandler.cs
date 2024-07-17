using HealthMed.Common.Exceptions;
using HealthMed.Patient.Domain.Contracts.Authentication;
using HealthMed.Patient.Domain.Contracts.Repositories;
using MediatR;

namespace HealthMed.Patient.Application.UseCases.Patient.AuthenticatePatient;

public class AuthenticatePatientHandler(IPatientRepository patientRepository, IPatientAuthentication patientAuthentication) : IRequestHandler<AuthenticatePatientRequest, AuthenticatePatientResponse>
{
    public async Task<AuthenticatePatientResponse> Handle(AuthenticatePatientRequest request, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.GetPatientUsingCpfAndEmail(request.Cpf, request.Email)
            ?? throw new ObjectNotFoundException("Não há nenhum paciente com essa combinação e e-mail e cpf.");

        var accessToken = await patientAuthentication.AuthenticatePatient(patient.Email, request.Password);

        return new AuthenticatePatientResponse(accessToken);
    }
}
