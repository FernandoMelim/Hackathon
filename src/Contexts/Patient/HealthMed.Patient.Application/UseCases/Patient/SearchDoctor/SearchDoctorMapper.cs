using AutoMapper;
using HealthMed.Patient.Domain.Entities;

namespace HealthMed.Patient.Application.UseCases.Patient.SearchDoctor;

public class SearchDoctorMapper : Profile
{
    public SearchDoctorMapper()
    {
        CreateMap<DoctorEntity, DoctorData>();
        CreateMap<DoctorAvailableAppointment, AppointmentData>();
    }
}
