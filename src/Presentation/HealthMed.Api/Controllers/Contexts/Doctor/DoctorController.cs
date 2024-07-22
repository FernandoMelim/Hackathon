using HealthMed.Api.Controllers.Base;
using HealthMed.Common.Validation;
using HealthMed.Doctor.Application.UseCases.Doctor.AcceptAppointment;
using HealthMed.Doctor.Application.UseCases.Doctor.AuthenticateDoctor;
using HealthMed.Doctor.Application.UseCases.Doctor.CreateMedicalAppointmentTime;
using HealthMed.Doctor.Application.UseCases.Doctor.GetPendingMedicalAppointments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.Api.Controllers.Contexts.Doctor;

[ApiController]
[Route("[controller]")]
public class DoctorController(ValidationNotifications validationNotifications, IMediator mediator) : BaseController(validationNotifications)
{
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

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiBaseResponse<GetPendingMedicalAppointmentsResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiBaseResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ApiBaseResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiBaseResponse))]
    [HttpGet("GetPendingMedicalAppointments/{doctorId}")]
    public async Task<IActionResult> GetPendingMedicalAppointments([FromRoute] int doctorId, CancellationToken cancellationToken)
    {
        var data = await mediator.Send(new GetPendingMedicalAppointmentsRequest(doctorId), cancellationToken);
        return await Return(new ApiBaseResponse<GetPendingMedicalAppointmentsResponse>() { Data = data });
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiBaseResponse<AcceptAppointmentResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiBaseResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ApiBaseResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiBaseResponse))]
    [HttpPatch("AcceptAppointment/{appointmentId}/{doctorAppointmentId}")]
    public async Task<IActionResult> AcceptAppointment([FromRoute] int appointmentId, [FromRoute] int doctorAppointmentId, CancellationToken cancellationToken)
    {
        var data = await mediator.Send(new AcceptAppointmentRequest(appointmentId, doctorAppointmentId), cancellationToken);
        return await Return(new ApiBaseResponse<AcceptAppointmentResponse>() { Data = data });
    }
}
