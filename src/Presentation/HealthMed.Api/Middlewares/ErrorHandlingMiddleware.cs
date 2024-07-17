using HealthMed.Api.Controllers.Base;
using HealthMed.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace HealthMed.Api.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        var result = new ApiBaseResponse();

        try
        {
            await next(context);
        }
        catch (ObjectNotFoundException ex)
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            result.StatusCode = HttpStatusCode.NotFound;
            result.Errors = new List<KeyValuePair<string, List<string>>>()
            {
                new KeyValuePair<string, List<string>>("ObjectNotFoundException", new List<string>() { "O item solicitado não foi encontrado" })
            };

            await response.WriteAsync(JsonSerializer.Serialize(result));
        }
        catch (ValidationException ex)
        {
            response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            result.StatusCode = HttpStatusCode.UnprocessableEntity;

            var errors = ex.ValidationNotifications.GetErrors();
            var errorsList = new List<KeyValuePair<string, List<string>>>();

            foreach (var error in errors)
                errorsList.Add(error);

            result.Errors = errorsList;

            await response.WriteAsync(JsonSerializer.Serialize(result));
        }
        catch (Exception ex)
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            result.StatusCode = HttpStatusCode.InternalServerError;
            result.Errors = new List<KeyValuePair<string, List<string>>>()
            {
                new KeyValuePair<string, List<string>>("InternalServerError", new List<string>() { "Internal server error" })
            };

            await response.WriteAsync(JsonSerializer.Serialize(result));
        }
    }
}

