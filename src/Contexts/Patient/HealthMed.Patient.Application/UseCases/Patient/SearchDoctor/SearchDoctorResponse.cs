using HealthMed.Patient.Domain.Entities;

namespace HealthMed.Patient.Application.UseCases.Patient.SearchDoctor;

public class SearchDoctorResponse
{
    public List<DoctorData> Doctors {  get; set; } = new List<DoctorData>();
}

public class DoctorData
{
    public int Id { get; set; }

    public string FullName { get; set; }

    public string Crm { get; set; }

    public string Email { get; set; }

    public string Address { get; set; }

    public int Rating { get; set; }

    public int ExpertiseId { get; set; }

    public string ExpertiseName { get; set; }

    public double DistanceInKm { get; set; }

    public string DistanceInTime { get; set; }

    public List<AppointmentData> AvailableAppointments { get; set; } = new List<AppointmentData>();
}

public class AppointmentData
{
    public int Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
