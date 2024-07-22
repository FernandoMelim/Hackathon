namespace HealthMed.Patient.Domain.Entities;

public class DoctorAvailableAppointment
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool Available { get; set; }
}
