using Dapper;
using HealthMed.Common.Database;
using HealthMed.Doctor.Domain.Contracts.Repositories;
using HealthMed.Doctor.Domain.Entitites;

namespace HealthMed.Doctor.Infrastructure.Database.Repositories;

public class DoctorRepository(Context context) : IDoctorRespository
{
    public async Task<int> AddAvailableAppointmentAsync(DoctorAvailableAppointment horarioDisponivel, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();

        return await connection.ExecuteScalarAsync<int>(@"
            INSERT INTO AVAILABLE_DOCTOR_APPOINTMENT(DOCTOR_ID, START_DATE, END_DATE, AVAILABLE)
            VALUES (@DoctorId, @StartDate, @EndDate, @Available)
            ", new { @DoctorId = horarioDisponivel.DoctorId, @StartDate = horarioDisponivel.StartDate, @EndDate = horarioDisponivel.EndDate, @Available = horarioDisponivel.Available }); ;

        throw new NotImplementedException();
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
}
