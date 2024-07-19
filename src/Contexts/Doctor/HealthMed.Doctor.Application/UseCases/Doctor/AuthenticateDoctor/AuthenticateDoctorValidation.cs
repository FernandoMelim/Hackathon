﻿using FluentValidation;
using System.Text.RegularExpressions;

namespace HealthMed.Doctor.Application.UseCases.Doctor.AuthenticateDoctor;

public class AuthenticateDoctorValidation : AbstractValidator<AuthenticateDoctorRequest>
{
    private static readonly string crmPattern = @"^\d{6}/[A-Z]{2}$";

    public AuthenticateDoctorValidation()
    {
        RuleFor(dto => dto.Crm)
           .NotEmpty()
           .WithMessage("O crm deve estar preenchido.")
           .Must(IsValidCRM)
           .WithMessage("O crm deve ser válido");

        RuleFor(dto => dto.Password)
            .NotEmpty()
            .WithMessage("A senha deve estar preenchida.")
            .Length(11, 11)
            .WithMessage("A senha deve ter 11 caracteres.");
    }

    public static bool IsValidCRM(string crm)
    {
        if (string.IsNullOrEmpty(crm))
            return false;
        
        crm = crm.Trim();

        return Regex.IsMatch(crm, crmPattern);
    }
}
