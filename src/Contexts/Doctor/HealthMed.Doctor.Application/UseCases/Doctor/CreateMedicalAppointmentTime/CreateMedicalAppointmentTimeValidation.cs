using Azure.Core;
using FluentValidation;
using HealthMed.Doctor.Application.UseCases.Doctor.AuthenticateDoctor;

namespace HealthMed.Doctor.Application.UseCases.Doctor.CreateMedicalAppointmentTime;

public class CreateMedicalAppointmentTimeValidation : AbstractValidator<CreateMedicalAppointmentTimeRequest>
{
    public CreateMedicalAppointmentTimeValidation()
    {
        RuleFor(dto => dto.startDate)
        .NotEmpty()
        .WithMessage("O crm deve estar preenchido.")
        .LessThan(dto => dto.endDate)
        .WithMessage("A data inicial deve ser menor que a final.");

        RuleFor(dto => dto.doctorId)
        .GreaterThan(0)
        .WithMessage("O id do médico deve estar preenchido.");
    }
}
