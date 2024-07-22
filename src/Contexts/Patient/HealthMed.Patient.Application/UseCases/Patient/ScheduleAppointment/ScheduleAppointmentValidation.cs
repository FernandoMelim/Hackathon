using FluentValidation;

namespace HealthMed.Patient.Application.UseCases.Patient.ScheduleAppointment;

public class ScheduleAppointmentValidation : AbstractValidator<ScheduleAppointmentRequest>
{
    public ScheduleAppointmentValidation()
    {
        RuleFor(dto => dto.patientId)
            .GreaterThan(0)
            .WithMessage("Insira um id de paciente válido");

        RuleFor(dto => dto.appointmentId)
            .GreaterThan(0)
            .WithMessage("Insira um id consultae válido");

    }
}
