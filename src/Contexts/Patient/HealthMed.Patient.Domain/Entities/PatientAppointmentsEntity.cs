namespace HealthMed.Patient.Domain.Entities;

public class PatientAppointmentsEntity
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public int IdAvailableDoctorAppointment { get; set; }
}
