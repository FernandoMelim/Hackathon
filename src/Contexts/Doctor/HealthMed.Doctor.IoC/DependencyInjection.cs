using Amazon.CognitoIdentityProvider;
using Amazon.Runtime;
using Dapper;
using FluentValidation;
using HealthMed.Common.Database;
using HealthMed.Common.Validation;
using HealthMed.Doctor.Domain.Contracts.Authentication;
using HealthMed.Doctor.Domain.Contracts.Repositories;
using HealthMed.Doctor.Infrastructure.Cognito;
using HealthMed.Doctor.Infrastructure.Database.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HealthMed.Doctor.IoC;

public static class DependencyInjection
{
    private static string pathToApplicationAssembly = Path.Combine(AppContext.BaseDirectory, "HealthMed.Doctor.dll");

    public static void ConfigureDoctorServices(this IServiceCollection services)
    {
        ConfigureCognito(services);
        ConfigureRepositories(services);
        ConfigureDatabase();
        ConfigureNotificationServices(services);
        ConfigureValidators(services);
        ConfigureMediatr(services);
        ConfigureAutomapper(services);
    }

    private static void ConfigureCognito(IServiceCollection services)
    {
        string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
        string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY");

        AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);

        var config = new AmazonCognitoIdentityProviderConfig();

        var cognitoProvider = new AmazonCognitoIdentityProviderClient(credentials, Amazon.RegionEndpoint.USEast1);

        services.AddSingleton(cognitoProvider);
        services.AddSingleton<IDoctorAuthentication, CognitoDoctorAuthentication>();
    }

    private static void ConfigureAutomapper(IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.LoadFrom(pathToApplicationAssembly));
    }

    private static void ConfigureDatabase()
    {
        var context = new Context();

        using var connection = context.CreateConnection();

        connection.Execute(@"", new { });
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
}
