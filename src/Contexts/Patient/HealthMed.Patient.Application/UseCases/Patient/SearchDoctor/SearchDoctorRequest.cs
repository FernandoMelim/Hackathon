using MediatR;

namespace HealthMed.Patient.Application.UseCases.Patient.SearchDoctor;

public record SearchDoctorRequest(int patientId, int? doctorExpertiseId, int? km, int? rating) : IRequest<SearchDoctorResponse>;
