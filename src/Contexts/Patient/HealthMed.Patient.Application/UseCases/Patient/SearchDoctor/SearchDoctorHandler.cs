using AutoMapper;
using HealthMed.Common.Exceptions;
using HealthMed.Patient.Domain.Contracts.Repositories;
using HealthMed.Patient.Domain.Entities;
using MediatR;
using System.Text.Json;

namespace HealthMed.Patient.Application.UseCases.Patient.SearchDoctor;

public class SearchDoctorHandler(IPatientRepository patientRepository, IMapper mapper) : IRequestHandler<SearchDoctorRequest, SearchDoctorResponse>
{
    public async Task<SearchDoctorResponse> Handle(SearchDoctorRequest request, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.GetPatientUsingId(request.patientId) 
            ?? throw new ObjectNotFoundException("Não há nenhum paciente com esse id.");

        var doctors = await patientRepository.GetDoctorByFilter(request.rating, request.doctorExpertiseId);
        var doctorsData = mapper.Map<List<DoctorData>>(doctors);

        if (request.km.HasValue)
            doctorsData = await CalculateDistance(doctorsData, request.km.Value, patient.Address);

        foreach(var doctor in doctorsData)
        {
            var appointments = await patientRepository.GetDoctorsAppointments(doctor.Id);
            doctor.AvailableAppointments = mapper.Map<List<AppointmentData>>(appointments);
        }

        return new SearchDoctorResponse() { Doctors = doctorsData };
    }

    private async Task<List<DoctorData>> CalculateDistance(List<DoctorData> doctors, int km, string patientAddres)
    {
        var filteredDoctors = new List<DoctorData>();

        foreach(var doctor in doctors)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://maps.googleapis.com");

            var response = await client.GetAsync($"maps/api/distancematrix/json?units=imperial&origins={patientAddres}&destinations={doctor.Address}&key={Environment.GetEnvironmentVariable("GCP_KEY")}");

            if(response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<DistanceMatrixResponse>(response.Content.ReadAsStringAsync().Result);
                if(result.Status != "DENIED")
                {
                    var distanceInKm = ConvertMetersToKilometers(result.Rows[0].Elements[0].Distance.Value);

                    if (distanceInKm <= km)
                    {
                        doctor.DistanceInKm = distanceInKm;
                        doctor.DistanceInTime = result.Rows[0].Elements[0].Duration.Text;
                        filteredDoctors.Add(doctor);
                    }
                }

            }
            else 
                filteredDoctors.Add(doctor);
        }

        return filteredDoctors;
    }

    public static double ConvertMetersToKilometers(double meters)
    {
        const double metersToKilometersFactor = 0.001;
        return meters * metersToKilometersFactor;
    }


}
