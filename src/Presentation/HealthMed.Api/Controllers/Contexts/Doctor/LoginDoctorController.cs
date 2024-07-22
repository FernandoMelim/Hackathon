using HealthMed.Api.Controllers.Base;
using HealthMed.Common.Validation;
using HealthMed.Doctor.Application.UseCases.Doctor.AuthenticateDoctor;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.Api.Controllers.Contexts.Doctor;

[ApiController]
[Route("[controller]")]
public class LoginDoctorController(ValidationNotifications validationNotifications, IMediator mediator) : BaseController(validationNotifications)
{
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiBaseResponse<AuthenticateDoctorResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiBaseResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiBaseResponse))]
    [HttpGet("AuthenticateDoctor")]
    public async Task<IActionResult> AuthenticateDoctor([FromQuery] AuthenticateDoctorRequest authenticateDoctorRequest, CancellationToken cancellationToken)
    {
        var data = await mediator.Send(authenticateDoctorRequest, cancellationToken);
        return await Return(new ApiBaseResponse<AuthenticateDoctorResponse>() { Data = data });
    }
}
