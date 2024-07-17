using FluentValidation;

namespace HealthMed.Patient.Application.UseCases.Patient.AuthenticatePatient;

public class AuthenticatePatientValidation : AbstractValidator<AuthenticatePatientRequest>
{
    public AuthenticatePatientValidation()
    {
        RuleFor(dto => dto.Email)
            .NotEmpty()
            .WithMessage("O e-mail deve estar preenchido.")
            .EmailAddress()
            .WithMessage("O e-mail fornecido não é válido.");

        RuleFor(dto => dto.Password)
            .NotEmpty()
            .WithMessage("A senha deve estar preenchida.")
            .Length(11, 11)
            .WithMessage("A senha deve ter 11 caracteres.");

        RuleFor(dto => dto.Cpf)
            .NotEmpty()
            .WithMessage("O cpf deve estar preenchido.")
            .Must(BeValidCpf)
            .WithMessage("O cpf deve ser válido");
    }

    public bool BeValidCpf(string cpf)
    {
        int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        string tempCpf;
        string digito;
        int soma;
        int resto;
        cpf = cpf.Trim();
        cpf = cpf.Replace(".", "").Replace("-", "");
        if (cpf.Length != 11)
            return false;
        tempCpf = cpf.Substring(0, 9);
        soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;
        digito = resto.ToString();
        tempCpf = tempCpf + digito;
        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;
        digito = digito + resto.ToString();

        return cpf.EndsWith(digito);
    }
}
