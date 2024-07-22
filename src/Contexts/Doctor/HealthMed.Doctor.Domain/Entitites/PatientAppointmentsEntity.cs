namespace HealthMed.Doctor.Domain.Entitites;

public class PatientAppointmentsEntity
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int IdAvailableDoctorAppointment { get; set; }
}
