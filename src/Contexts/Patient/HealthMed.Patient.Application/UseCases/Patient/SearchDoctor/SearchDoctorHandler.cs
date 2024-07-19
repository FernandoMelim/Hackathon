using HealthMed.Common.Exceptions;
using HealthMed.Patient.Domain.Contracts.Repositories;
using HealthMed.Patient.Domain.Entities;
using MediatR;

namespace HealthMed.Patient.Application.UseCases.Patient.SearchDoctor;

public class SearchDoctorHandler(IPatientRepository patientRepository) : IRequestHandler<SearchDoctorRequest, SearchDoctorResponse>
{
    public async Task<SearchDoctorResponse> Handle(SearchDoctorRequest request, CancellationToken cancellationToken)
    {
        var patient = patientRepository.GetPatientUsingId(request.patientId) 
            ?? throw new ObjectNotFoundException("Não há nenhum paciente com esse id.");

        var doctors = await patientRepository.GetDoctorByFilter(request.rating, request.doctorExpertiseId);

        return new SearchDoctorResponse() { Doctors = doctors };
    }
}
