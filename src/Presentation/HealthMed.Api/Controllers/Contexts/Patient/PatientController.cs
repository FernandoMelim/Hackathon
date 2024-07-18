using HealthMed.Api.Controllers.Base;
using HealthMed.Common.Validation;
using HealthMed.Patient.Application.UseCases.Patient.AuthenticatePatient;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.Api.Controllers.Contexts.Patient;

[ApiController]
[Route("[controller]")]
public class PatientController(ValidationNotifications validationNotifications, IMediator mediator) : BaseController(validationNotifications)
{
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiBaseResponse<AuthenticatePatientResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiBaseResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiBaseResponse))]
    [HttpGet("AuthenticatePatient")]
    public async Task<IActionResult> AuthenticatePatient([FromQuery] AuthenticatePatientRequest authenticatePatientRequest, CancellationToken cancellationToken)
    {
        var data = await mediator.Send(authenticatePatientRequest, cancellationToken);
        return await Return(new ApiBaseResponse<AuthenticatePatientResponse>() { Data = data });
    }
}
