using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Dapper;
using FluentValidation;
using HealthMed.Common.Validation;
using HealthMed.Patient.Domain.Contracts.Authentication;
using HealthMed.Patient.Domain.Contracts.Repositories;
using HealthMed.Patient.Domain.Entities;
using HealthMed.Patient.Infrastructure.Cognito;
using HealthMed.Patient.Infrastructure.Database.Context;
using HealthMed.Patient.Infrastructure.Database.Repositories;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HealthMed.Patient.IoC;

public static class DependencyInjection
{
    private static string pathToApplicationAssembly = Path.Combine(AppContext.BaseDirectory, "HealthMed.Patient.Application.dll");

    public static void ConfigurePatientServices(this IServiceCollection services)
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

        var config = new AmazonCognitoIdentityProviderConfig();

        var cognitoProvider = new AmazonCognitoIdentityProviderClient(credentials, Amazon.RegionEndpoint.USEast1);

        services.AddSingleton(cognitoProvider);
        services.AddSingleton<IPatientAuthentication, CognitoPatientAuthentication>();

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
    => services.AddSingleton<IPatientRepository, PatientRepository>();

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

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PATIENTS' AND xtype='U')
            BEGIN
                CREATE TABLE PATIENTS(
                    ID INT NOT NULL IDENTITY(1,1),
                    FULLNAME VARCHAR(MAX) NOT NULL,
                    CPF VARCHAR(11),
                    EMAIL VARCHAR(MAX)  
                );
            END

        ", new { });

        var patients = new List<PatientEntity>()
        {
            new PatientEntity(){ FullName = "Sérgio", Cpf = "70561391084", Email = "sergio@sergio.sergio" },
            new PatientEntity(){ FullName = "Fernando", Cpf = "33590050071", Email = "fernando@fernando.com" },
            new PatientEntity(){ FullName = "João", Cpf = "93211266003", Email = "joao@joao.com" },
            new PatientEntity(){ FullName = "Maria", Cpf = "69147373040", Email = "maria.maria@maria.com" },
            new PatientEntity(){ FullName = "Jasmine", Cpf = "70633353086", Email = "jasmine@jasmine.com" },
            new PatientEntity(){ FullName = "Alice", Cpf = "36458377010", Email = "alice@alice.com" },
        };

        foreach (var patient in patients)
        {
            await InsertPatientIfNotExistsAsync(patient);
            await RegisterUserInCognitoIfNotExistsAsync(patient.Email, cognitoProvider);
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

    private static async Task InsertPatientIfNotExistsAsync(PatientEntity patient)
    {
        var context = new Context();

        using var connection = context.CreateConnection();

        var checkQuery = "SELECT COUNT(1) FROM Patients WHERE Email = @Email";
        var exists = await connection.ExecuteScalarAsync<int>(checkQuery, new { Email = patient.Email }) > 0;

        if (!exists)
        {
            var insertQuery = "INSERT INTO Patients (FullName, Cpf, Email) VALUES (@FullName, @Cpf, @Email)";
            await connection.ExecuteAsync(insertQuery, patient);
        }
    }

    private static async Task RegisterUserInCognitoIfNotExistsAsync(string email, AmazonCognitoIdentityProviderClient cognitoProvider)
    {
        var userPoolId = Environment.GetEnvironmentVariable("AWS_PATIENT_POOL_ID");

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
}
