using FluentValidation;

namespace HealthMed.Doctor.Application.UseCases.Doctor.AcceptAppointment;

public class AcceptAppointmentValidator : AbstractValidator<AcceptAppointmentRequest>
{
    public AcceptAppointmentValidator()
    {
        RuleFor(dto => dto.appointmentId)
            .GreaterThan(0)
            .WithMessage("O id da consulta deve ser válido.");

        RuleFor(dto => dto.doctorAppointmentId)
          .GreaterThan(0)
          .WithMessage("O id da consulta do doutor deve ser válido.");
    }
}
