using HealthMed.Api.Controllers.Base;
using HealthMed.Common.Validation;
using HealthMed.Patient.Application.UseCases.Patient.AuthenticatePatient;
using HealthMed.Patient.Application.UseCases.Patient.ScheduleAppointment;
using HealthMed.Patient.Application.UseCases.Patient.SearchDoctor;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.Api.Controllers.Contexts.Patient;

[ApiController]
[Route("[controller]")]
public class PatientController(ValidationNotifications validationNotifications, IMediator mediator) : BaseController(validationNotifications)
{
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiBaseResponse<SearchDoctorResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiBaseResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiBaseResponse))]
    [HttpGet("SearchDoctor")]
    public async Task<IActionResult> SearchDoctor([FromQuery] int patientId, [FromQuery] int? rating, [FromQuery] int? doctorExpertiseId, [FromQuery] int? km, CancellationToken cancellationToken)
    {
        var data = await mediator.Send(new SearchDoctorRequest(patientId, doctorExpertiseId, km, rating), cancellationToken);
        return await Return(new ApiBaseResponse<SearchDoctorResponse>() { Data = data });
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiBaseResponse<ScheduleAppointmentResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiBaseResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiBaseResponse))]
    [HttpPost("ScheduleAppointment")]
    public async Task<IActionResult> ScheduleAppointment([FromBody] ScheduleAppointmentRequest appointmentRequest, CancellationToken cancellationToken)
    {
        var data = await mediator.Send(appointmentRequest, cancellationToken);
        return await Return(new ApiBaseResponse<ScheduleAppointmentResponse>() { Data = data });
    }
}
