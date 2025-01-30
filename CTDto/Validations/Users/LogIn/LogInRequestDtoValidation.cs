using CTDto.Users.LogIn;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Users.LogIn
{
    public class LogInRequestDtoValidation : AbstractValidator<LoginRequestDto>
    {
        public LogInRequestDtoValidation()
        {
            RuleFor(x => x.Fullname)
                .NotEmpty().WithMessage("El nombre de usuario es requerido.")
                .Length(3, 50).WithMessage("El nombre de usuario debe tener entre 3 y 50 caracteres.");

            RuleFor(x => x.Passcode)
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .Length(3, 15).WithMessage("La contraseña debe tener entre 3 y 15 caracteres.");
        }

    }
}
