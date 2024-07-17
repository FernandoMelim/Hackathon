using HealthMed.Common.Validation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HealthMed.Api.Controllers.Base;

public class BaseController(ValidationNotifications validationNotifications) : ControllerBase
{
    protected async virtual Task<IActionResult> Return<T>(ApiBaseResponse<T> apiBaseResponse, int? statusCode = null)
    {
        if (!validationNotifications.HasErrors())
        {
            apiBaseResponse.StatusCode = (HttpStatusCode?)statusCode ?? HttpStatusCode.OK;
            return await Return(apiBaseResponse, (int)apiBaseResponse.StatusCode);
        }

        var errors = validationNotifications.GetErrors();
        apiBaseResponse.Errors = new List<KeyValuePair<string, List<string>>>();

        foreach (var error in errors)
            apiBaseResponse.Errors.Add(error);

        apiBaseResponse.StatusCode = (HttpStatusCode?)statusCode ?? HttpStatusCode.UnprocessableEntity;

        return await Return(apiBaseResponse, (int)apiBaseResponse.StatusCode);
    }

    private async Task<IActionResult> Return<T>(ApiBaseResponse<T> apiBaseResponse, int statusCode)
    {
        switch (statusCode)
        {
            case (int)HttpStatusCode.OK:
                return Ok(apiBaseResponse);
            case (int)HttpStatusCode.NoContent:
                return NoContent();
            case (int)HttpStatusCode.UnprocessableEntity:
                return UnprocessableEntity(apiBaseResponse);
            case (int)HttpStatusCode.InternalServerError:
                return StatusCode((int)HttpStatusCode.InternalServerError);
            default:
                return BadRequest();
        }
    }
}
