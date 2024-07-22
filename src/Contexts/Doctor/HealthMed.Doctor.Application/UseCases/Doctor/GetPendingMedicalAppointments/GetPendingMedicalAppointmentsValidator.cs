using FluentValidation;

namespace HealthMed.Doctor.Application.UseCases.Doctor.GetPendingMedicalAppointments;

public class GetPendingMedicalAppointmentsValidator : AbstractValidator<GetPendingMedicalAppointmentsRequest>
{
    public GetPendingMedicalAppointmentsValidator()
    {
        RuleFor(dto => dto.doctorId)
        .GreaterThan(0)
        .WithMessage("Digite um valor válido para o doctorId.");
    }
}
