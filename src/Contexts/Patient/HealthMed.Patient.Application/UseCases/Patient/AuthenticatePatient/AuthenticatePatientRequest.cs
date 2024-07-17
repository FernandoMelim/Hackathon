using MediatR;

namespace HealthMed.Patient.Application.UseCases.Patient.AuthenticatePatient;

public record AuthenticatePatientRequest(string Email, string Cpf, string Password) : IRequest<AuthenticatePatientResponse>;