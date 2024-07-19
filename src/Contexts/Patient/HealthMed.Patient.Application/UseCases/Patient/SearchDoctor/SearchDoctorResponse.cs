using HealthMed.Patient.Domain.Entities;

namespace HealthMed.Patient.Application.UseCases.Patient.SearchDoctor;

public class SearchDoctorResponse
{
    public List<DoctorEntity> Doctors {  get; set; } = new List<DoctorEntity>();
}
