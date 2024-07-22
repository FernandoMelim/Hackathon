using Dapper;
using HealthMed.Patient.Domain.Contracts.Repositories;
using HealthMed.Patient.Domain.Entities;

namespace HealthMed.Patient.Infrastructure.Database.Repositories;

public class PatientRepository(Context.Context context) : IPatientRepository
{
    public async Task<List<DoctorEntity>> GetDoctorByFilter(int? rating, int? doctorExpertiseId)
    {
        using var connection = context.CreateConnection();

        var sql = @"            
            SELECT 
                d.ID as Id,
                D.FULLNAME AS FullName,
                D.CRM AS Crm,
                D.EMAIL AS Email,
                D.ADDRESS AS Address,
                D.EXPERTISE_ID as ExpertiseId,
                D.RATING AS Rating,
                DC.EXPERTISE as ExpertiseName
            FROM DOCTORS D
            JOIN DOCTOR_EXPERTISE DC ON D.EXPERTISE_ID = DC.ID
            WHERE 1=1 ";

        if(rating.HasValue)
        {
            sql += @" AND D.RATING >= @rating ";
        }

        if(doctorExpertiseId.HasValue)
        {
            sql += @" AND D.EXPERTISE_ID = @doctorExpertiseId ";
        }


        return (await connection.QueryAsync<DoctorEntity>(sql, new { @rating = rating, @doctorExpertiseId = doctorExpertiseId })).ToList();
    }

    public async Task<PatientEntity> GetPatientUsingCpfAndEmail(string cpf, string email)
    {
        using var connection = context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<PatientEntity>(@"
            SELECT 
                FULLNAME AS FullName,
                CPF AS Cpf,
                EMAIL AS Email,
                ADRESS as Address
            FROM PATIENTS
            WHERE EMAIL = @email 
                AND CPF = @cpf;
            ", new { @email = email, @cpf = cpf });

    }

    public async Task<PatientEntity> GetPatientUsingId(int id)
    {
        using var connection = context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<PatientEntity>(@"
            SELECT 
                ID AS Id,
                FULLNAME AS FullName,
                CPF AS Cpf,
                EMAIL AS Email,
                ADDRESS as Address
            FROM PATIENTS
            WHERE ID = @id;
            ", new { @id = id });
    }
}
