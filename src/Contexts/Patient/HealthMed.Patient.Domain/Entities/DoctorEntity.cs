namespace HealthMed.Patient.Domain.Entities;

public class DoctorEntity
{
    public int Id { get; set; }

    public string FullName { get; set; }

    public string Crm { get; set; }

    public string Email { get; set; }

    public string Address { get; set; }

    public int Rating { get; set; }

    public int ExpertiseId { get; set; }
    public string ExpertiseName { get; set; }
}
