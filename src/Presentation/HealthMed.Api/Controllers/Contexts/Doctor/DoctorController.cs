using HealthMed.Api.Controllers.Base;
using HealthMed.Common.Validation;
using HealthMed.Doctor.Application.UseCases.Doctor.AuthenticateDoctor;
using HealthMed.Doctor.Application.UseCases.Doctor.CreateMedicalAppointmentTime;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.Api.Controllers.Contexts.Doctor;

[ApiController]
[Route("[controller]")]
public class DoctorController(ValidationNotifications validationNotifications, IMediator mediator) : BaseController(validationNotifications)
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

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiBaseResponse<CreateMedicalAppointmentTimeResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiBaseResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ApiBaseResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiBaseResponse))]
    [HttpPost("CreateMedicalAppointmentTime")]
    public async Task<IActionResult> CreateMedicalAppointmentTime([FromBody] CreateMedicalAppointmentTimeRequest createMedicalAppointmentTimeRequest, CancellationToken cancellationToken)
    {
        var data = await mediator.Send(createMedicalAppointmentTimeRequest, cancellationToken);
        return await Return(new ApiBaseResponse<CreateMedicalAppointmentTimeResponse>() { Data = data });
    }
}
