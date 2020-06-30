using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Extensions
{
    public static class ValidatorExtension
    {
        public static IRuleBuilder<T, string> MobileNumberInKazakhstan<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .Length(11)
                .Matches("^7+([0-9]){10}$")
                .WithMessage("'{PropertyName}' is not valid phone number");
        }
    }
}
