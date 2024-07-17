using Microsoft.Data.SqlClient;
using System.Data;

namespace HealthMed.Patient.Infrastructure.Database.Context;

public class Context
{
    private readonly string _connectionString;

    public Context()
       => _connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION") ?? throw new ArgumentNullException("Empty connection string");

    public virtual IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);
}
