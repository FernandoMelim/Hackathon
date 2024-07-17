using Dapper;
using HealthMed.Patient.Domain.Contracts.Repositories;
using HealthMed.Patient.Domain.Entities;

namespace HealthMed.Patient.Infrastructure.Database.Repositories;

public class PatientRepository(Context.Context context) : IPatientRepository
{
    public async Task<PatientEntity> GetPatientUsingCpfAndEmail(string cpf, string email)
    {
        using var connection = context.CreateConnection();

        return await connection.QueryFirstAsync<PatientEntity>(@"
            SELECT 
                FULLNAME AS FullName,
                CPF AS Cpf,
                EMAIL AS Email
            FROM PATIENTS
            WHERE EMAIL = @email 
                AND CPF = @cpf;
            ", new { @email = email, @cpf = cpf });

    }
}
