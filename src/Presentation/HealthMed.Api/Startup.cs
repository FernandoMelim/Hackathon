using Amazon;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.Runtime;
using HealthMed.Api.Middlewares;
using HealthMed.Doctor.IoC;
using HealthMed.Patient.IoC;
using NLog;
using NLog.AWS.Logger;
using NLog.Config;

namespace HealthMed.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

        services.ConfigureDoctorServices();
        services.ConfigurePatientServices();
        ConfigureCloudWatch(services);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }

    private async void ConfigureCloudWatch(IServiceCollection services)
    {
        string accessKey = Environment.GetEnvironmentVariable("ACCESS_KEY");
        string secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

        AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);

        var config = new LoggingConfiguration();

        config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, new AWSTarget()
        {
            LogGroup = Environment.GetEnvironmentVariable("LOG_GROUP"),
            Region = "us-east-1",
            Credentials = credentials
        });


        LogManager.Configuration = config;

        var log = LogManager.GetCurrentClassLogger();

        services.AddSingleton(log);
    }

}