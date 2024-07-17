using Dapper;
using HealthMed.Common.Database;
using HealthMed.Doctor.Domain.Contracts.Repositories;
using HealthMed.Doctor.Domain.Entitites;

namespace HealthMed.Doctor.Infrastructure.Database.Repositories;

public class DoctorRepository(Context context) : IDoctorRespository
{
    public async Task<DoctorEntity> GetDoctorByCrm(string crm)
    {
        using var connection = context.CreateConnection();

        return await connection.QueryFirstAsync<DoctorEntity>(@"
            SELECT 
                FULLNAME AS FullName,
                CRM AS Crm,
                EMAIL AS Email
            FROM DOCTORS
            WHERE CRM = @crm;
            ", new { @crm = crm });
    }
}
