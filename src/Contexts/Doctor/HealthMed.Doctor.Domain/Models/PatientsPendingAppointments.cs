namespace HealthMed.Doctor.Domain.Models;

public class PatientsPendingAppointments
{
    public int DoctorAvailableAppointmentId { get; set; }
    public int DoctorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PendingAppointmentId { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; }
}
