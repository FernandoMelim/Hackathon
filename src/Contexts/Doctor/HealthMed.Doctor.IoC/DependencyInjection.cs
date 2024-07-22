using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Azure.Core;
using Dapper;
using FluentValidation;
using HealthMed.Common.Database;
using HealthMed.Common.Validation;
using HealthMed.Doctor.Domain.Contracts.Authentication;
using HealthMed.Doctor.Domain.Contracts.Repositories;
using HealthMed.Doctor.Domain.Entitites;
using HealthMed.Doctor.Infrastructure.Cognito;
using HealthMed.Doctor.Infrastructure.Database.Repositories;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading;

namespace HealthMed.Doctor.IoC;

public static class DependencyInjection
{
    private static string pathToApplicationAssembly = Path.Combine(AppContext.BaseDirectory, "HealthMed.Doctor.Application.dll");

    public static void ConfigureDoctorServices(this IServiceCollection services)
    {
        var cognitoProvider = ConfigureCognito(services);
        ConfigureRepositories(services);
        ConfigureNotificationServices(services);
        ConfigureValidators(services);
        ConfigureMediatr(services);
        ConfigureAutomapper(services);
        ConfigureDatabase(services, cognitoProvider);
    }

    private static AmazonCognitoIdentityProviderClient ConfigureCognito(IServiceCollection services)
    {
        string accessKey = Environment.GetEnvironmentVariable("ACCESS_KEY");
        string secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

        AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);

        var cognitoProvider = new AmazonCognitoIdentityProviderClient(credentials, Amazon.RegionEndpoint.USEast1);

        services.AddSingleton(cognitoProvider);
        services.AddSingleton<IDoctorAuthentication, CognitoDoctorAuthentication>();

        return cognitoProvider;
    }

    private static void ConfigureAutomapper(IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.LoadFrom(pathToApplicationAssembly));
    }

    private static void ConfigureMediatr(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.LoadFrom(pathToApplicationAssembly)));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }

    private static void ConfigureRepositories(IServiceCollection services)
    => services.AddSingleton<IDoctorRespository, DoctorRepository>();

    private static void ConfigureNotificationServices(IServiceCollection services)
        => services.AddScoped<ValidationNotifications>();


    private static void ConfigureValidators(IServiceCollection services)
        => services.AddValidatorsFromAssembly(Assembly.LoadFrom(pathToApplicationAssembly));

    private static async void ConfigureDatabase(IServiceCollection services, AmazonCognitoIdentityProviderClient cognitoProvider)
    {
        CriarDb();
        services.AddSingleton<Context>();

        var context = new Context();

        using var connection = context.CreateConnection();

        connection.Execute(@"

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DOCTOR_EXPERTISE' AND xtype='U')
                BEGIN
                    CREATE TABLE DOCTOR_EXPERTISE(
                        ID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
                        EXPERTISE VARCHAR(MAX) NOT NULL
                    );
                END

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DOCTORS' AND xtype='U')
                BEGIN
                    CREATE TABLE DOCTORS(
                        ID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
                        FULLNAME VARCHAR(MAX) NOT NULL,
                        CRM VARCHAR(9),
                        EMAIL VARCHAR(MAX) NOT NULL,
                        ADDRESS VARCHAR(MAX) NOT NULL,
                        RATING INT NOT NULL,
                        EXPERTISE_ID INT NOT NULL,
                        CONSTRAINT FK_DOCTORS_EXPERTISE FOREIGN KEY (EXPERTISE_ID) REFERENCES DOCTOR_EXPERTISE(ID)
                    );
                END

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AVAILABLE_DOCTOR_APPOINTMENT' AND xtype='U')
                BEGIN
                    CREATE TABLE AVAILABLE_DOCTOR_APPOINTMENT (
                        ID INT IDENTITY(1,1) PRIMARY KEY,
                        DOCTOR_ID INT NOT NULL,
                        START_DATE DATETIME NOT NULL,
                        END_DATE DATETIME NOT NULL,
                        AVAILABLE BIT NOT NULL,
                        CONSTRAINT FK_HorariosDisponiveis_Medicos FOREIGN KEY (DOCTOR_ID)
                            REFERENCES DOCTORS (ID)
                    );
                END

        ", new { });


        var expertises = new List<DoctorExpertiseEntity>()
        {
            new DoctorExpertiseEntity(){ Expertise = "Dermatologista" },
            new DoctorExpertiseEntity(){ Expertise = "Ortopedista" },
            new DoctorExpertiseEntity(){ Expertise = "Ginecologista" },
            new DoctorExpertiseEntity(){ Expertise = "Cardiologista" },
            new DoctorExpertiseEntity(){ Expertise = "Endocrinologista" },
            new DoctorExpertiseEntity(){ Expertise = "Neurologista" },
        };


        var idsExpertise = new List<int>();
        foreach (var expertise in expertises)
        {
            var id = await InsertExpertiseIfNotExistsAsync(expertise);
            idsExpertise.Add(id);
        }

        var doctors = new List<DoctorEntity>()
        {
            new DoctorEntity(){ FullName = "Carlos", Crm = "123456/MG", Email = "carlos@exemplo.com", ExpertiseId = idsExpertise[0], Rating = 5, Address = "Rua Sampaio, 87, Juiz de Fora, MG" },
            new DoctorEntity(){ FullName = "Ana", Crm = "123456/MG", Email = "ana@exemplo.com", ExpertiseId = idsExpertise[1], Rating = 4, Address = "Rua Sampaio, 87, Juiz de Fora, MG" },
            new DoctorEntity(){ FullName = "Bruno", Crm = "123456/MG", Email = "bruno@exemplo.com", ExpertiseId = idsExpertise[2], Rating = 3, Address = "Rua Sampaio, 87, Juiz de Fora, MG" },
            new DoctorEntity(){ FullName = "Patrícia", Crm = "123456/MG", Email = "patricia@exemplo.com", ExpertiseId = idsExpertise[3], Rating = 2, Address = "R. 15 - Salutaris, Paraíba do Sul - RJ, Brazil" },
            new DoctorEntity(){ FullName = "Leonardo", Crm = "123456/MG", Email = "leonardo@exemplo.com", ExpertiseId = idsExpertise[4] , Rating = 1, Address = "R. 15 - Salutaris, Paraíba do Sul - RJ, Brazil" },
            new DoctorEntity(){ FullName = "Clara", Crm = "123456/MG", Email = "clara@exemplo.com", ExpertiseId = idsExpertise[5], Rating = 4, Address = "R. 15 - Salutaris, Paraíba do Sul - RJ, Brazil" }
        };

        foreach (var doctor in doctors)
        {
            var doctorId = await InsertDoctorIfNotExistsAsync(doctor);
            await RegisterUserInCognitoIfNotExistsAsync(doctor.Email, cognitoProvider);
        }
    }

    private static async Task<int> InsertExpertiseIfNotExistsAsync(DoctorExpertiseEntity expertise)
    {
        var context = new Context();

        using var connection = context.CreateConnection();

        var checkQuery = "SELECT ID FROM DOCTOR_EXPERTISE WHERE EXPERTISE = @EXPERTISE";
        var existingId = await connection.ExecuteScalarAsync<int?>(checkQuery, new { EXPERTISE = expertise.Expertise });

        if (existingId.HasValue)
        {
            return existingId.Value;
        }
        else
        {
            var insertQuery = "INSERT INTO DOCTOR_EXPERTISE (EXPERTISE) OUTPUT INSERTED.ID VALUES (@EXPERTISE)";
            var newId = await connection.ExecuteScalarAsync<int>(insertQuery, new { EXPERTISE = expertise.Expertise });
            return newId;
        }
    }

    private static async Task<int> InsertDoctorIfNotExistsAsync(DoctorEntity doctor)
    {
        var context = new Context();

        using var connection = context.CreateConnection();

        var checkQuery = "SELECT COUNT(1) FROM DOCTORS WHERE Email = @Email";
        var exists = await connection.ExecuteScalarAsync<int>(checkQuery, new { Email = doctor.Email }) > 0;

        if (!exists)
        {
            var insertQuery = "INSERT INTO DOCTORS (FullName, Crm, Email, ADDRESS, RATING, EXPERTISE_ID) OUTPUT INSERTED.ID VALUES (@FullName, @Crm, @Email, @Address, @Rating, @ExpertiseId) ";
            return await connection.ExecuteScalarAsync<int>(insertQuery, doctor);
        }

        return 0;
    }

    private static async Task RegisterUserInCognitoIfNotExistsAsync(string email, AmazonCognitoIdentityProviderClient cognitoProvider)
    {
        var userPoolId = Environment.GetEnvironmentVariable("AWS_DOCTOR_POOL_ID");

        try
        {
            var getUserRequest = new AdminGetUserRequest
            {
                UserPoolId = userPoolId,
                Username = email
            };

            await cognitoProvider.AdminGetUserAsync(getUserRequest);
        }
        catch (UserNotFoundException)
        {
            var createUserRequest = new AdminCreateUserRequest
            {
                UserPoolId = userPoolId,
                Username = email
            };

            await cognitoProvider.AdminCreateUserAsync(createUserRequest);

            var setPasswordRequest = new AdminSetUserPasswordRequest
            {
                Password = "12345678911",
                Username = email,
                UserPoolId = userPoolId,
                Permanent = true
            };

            await cognitoProvider.AdminSetUserPasswordAsync(setPasswordRequest);
        }
    }


    private static void CriarDb()
    {
        string serverConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_WITHOUT_DB");
        string databaseName = "HealthMed";

        using (var connection = new SqlConnection(serverConnectionString))
        {
            connection.Open();

            var databaseExists = connection.QueryFirstOrDefault<int>(
                $"SELECT COUNT(*) FROM sys.databases WHERE name = @databaseName",
                new { databaseName });

            if (databaseExists == 0)
            {
                var createDatabaseSql = $"CREATE DATABASE [{databaseName}]";
                connection.Execute(createDatabaseSql);
            }
        }
    }
}
