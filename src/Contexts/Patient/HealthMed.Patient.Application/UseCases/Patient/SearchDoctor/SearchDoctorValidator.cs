using FluentValidation;

namespace HealthMed.Patient.Application.UseCases.Patient.SearchDoctor;

public class SearchDoctorValidator : AbstractValidator<SearchDoctorRequest>
{
    public SearchDoctorValidator()
    {
        RuleFor(dto => dto.patientId)
            .NotEqual(0)
            .WithMessage("O patientId deve ser válido.");
    }
}
