using Amazon.CognitoIdentityProvider;
using Amazon.Runtime;
using Dapper;
using FluentValidation;
using HealthMed.Common.Validation;
using HealthMed.Patient.Domain.Contracts.Authentication;
using HealthMed.Patient.Domain.Contracts.Repositories;
using HealthMed.Patient.Infrastructure.Cognito;
using HealthMed.Patient.Infrastructure.Database.Context;
using HealthMed.Patient.Infrastructure.Database.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HealthMed.Patient.IoC;

public static class DependencyInjection
{
    private static string pathToApplicationAssembly = Path.Combine(AppContext.BaseDirectory, "HealthMed.Patient.Application.dll");

    public static void ConfigurePatientServices(this IServiceCollection services)
    {
        ConfigureCognito(services);
        ConfigureRepositories(services);
        ConfigureDatabase(services);
        ConfigureNotificationServices(services);
        ConfigureValidators(services);
        ConfigureMediatr(services);
        ConfigureAutomapper(services);
    }

    private static void ConfigureCognito(IServiceCollection services)
    {
        string accessKey = Environment.GetEnvironmentVariable("ACCESS_KEY");
        string secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

        AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);

        var config = new AmazonCognitoIdentityProviderConfig();

        var cognitoProvider = new AmazonCognitoIdentityProviderClient(credentials, Amazon.RegionEndpoint.USEast1);

        services.AddSingleton(cognitoProvider);
        services.AddSingleton<IPatientAuthentication, CognitoPatientAuthentication>();
    }

    private static void ConfigureAutomapper(IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.LoadFrom(pathToApplicationAssembly));
    }

    private static void ConfigureDatabase(IServiceCollection services)
    {
        services.AddSingleton<Context>();
        //var context = new Context();

        //using var connection = context.CreateConnection();

        //connection.Execute(@"", new { });
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
}
