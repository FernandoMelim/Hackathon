using MediatR;

namespace HealthMed.Doctor.Application.UseCases.Doctor.AuthenticateDoctor;

public record AuthenticateDoctorRequest(string Crm, string Password) : IRequest<AuthenticateDoctorResponse>;