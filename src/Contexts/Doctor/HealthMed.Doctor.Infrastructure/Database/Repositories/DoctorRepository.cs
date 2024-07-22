using Dapper;
using HealthMed.Common.Database;
using HealthMed.Doctor.Domain.Contracts.Repositories;
using HealthMed.Doctor.Domain.Entitites;
using HealthMed.Doctor.Domain.Models;

namespace HealthMed.Doctor.Infrastructure.Database.Repositories;

public class DoctorRepository(Context context) : IDoctorRespository
{
    public async Task AcceptPendingMedicalAppointment(int appointmentId, int doctorAppointmentId)
    {
        using var connection = context.CreateConnection();

        await connection.ExecuteAsync(@"
                UPDATE PATIENT_APPOINTMENTS
                SET ACCEPTED = 0
                WHERE ID_AVAILABLE_DOCTOR_APPOINTMENT = @doctorAppointmentId;

                UPDATE PATIENT_APPOINTMENTS
                SET ACCEPTED = 1
                WHERE ID = @appointmentId;

                UPDATE AVAILABLE_DOCTOR_APPOINTMENT
                SET AVAILABLE = 0
                WHERE ID = @doctorAppointmentId;
                
            "
            , new { @appointmentId = appointmentId, @doctorAppointmentId = doctorAppointmentId });
    }

    public async Task<int> AddAvailableAppointmentAsync(DoctorAvailableAppointment horarioDisponivel, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();

        return await connection.ExecuteScalarAsync<int>(@"
            INSERT INTO AVAILABLE_DOCTOR_APPOINTMENT(DOCTOR_ID, START_DATE, END_DATE, AVAILABLE)
            VALUES (@DoctorId, @StartDate, @EndDate, @Available)
            ", new { @DoctorId = horarioDisponivel.DoctorId, @StartDate = horarioDisponivel.StartDate, @EndDate = horarioDisponivel.EndDate, @Available = horarioDisponivel.Available }); ;
    }

    public async Task<DoctorEntity> GetByIdAsync(int doctorId, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<DoctorEntity>(@"
            SELECT 
                FULLNAME AS FullName,
                CRM AS Crm,
                EMAIL AS Email,
                ADDRESS AS Address,
                EXPERTISE_ID as ExpertiseId,
                RATING AS Rating,
                ID AS Id
            FROM DOCTORS
            WHERE ID = @doctorId;
            ", new { @doctorId = doctorId });
    }

    public async Task<DoctorEntity> GetDoctorByCrm(string crm)
    {
        using var connection = context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<DoctorEntity>(@"
            SELECT 
                FULLNAME AS FullName,
                CRM AS Crm,
                EMAIL AS Email,
                ADDRESS AS Address,
                EXPERTISE_ID as ExpertiseId,
                RATING AS Rating,
                ID AS Id
            FROM DOCTORS
            WHERE CRM = @crm;
            ", new { @crm = crm });
    }

    public async Task<List<PatientsPendingAppointments>> GetPendingAppointments(int doctorId)
    {
        using var connection = context.CreateConnection();

        return (await connection.QueryAsync<PatientsPendingAppointments>(@"
            SELECT 
                ADA.ID AS DoctorAvailableAppointmentId,
                ADA.DOCTOR_ID AS DoctorId,
                ADA.START_DATE AS StartDate,
                ADA.END_DATE AS EndDate,
                PA.ID AS PendingAppointmentId,
                PA.PATIENT_ID AS PatientId,
                P.FULLNAME AS PatientName
            FROM AVAILABLE_DOCTOR_APPOINTMENT ADA
            JOIN PATIENT_APPOINTMENTS PA ON ADA.ID = PA.ID_AVAILABLE_DOCTOR_APPOINTMENT
            JOIN PATIENTS P ON PA.PATIENT_ID = P.ID
            WHERE 
                ADA.DOCTOR_ID = @doctorId 
                AND ADA.AVAILABLE = 1 
                AND PA.ACCEPTED = 0;
            ", new { @doctorId = doctorId })).ToList();
    }
}
