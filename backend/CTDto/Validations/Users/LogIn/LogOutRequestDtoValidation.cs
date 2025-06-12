using CTDto.Users.LogIn;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Users.LogIn
{
    public class LogOutRequestDtoValidation : AbstractValidator<LogOutDto>
    {
        public LogOutRequestDtoValidation()
        {
            RuleFor(x => x.RefreshToken)
           .NotEmpty().WithMessage("El refresh token es requerido.")
           .Must(BeAValidGuid).WithMessage("El refresh token debe ser un GUID válido.");
        }

        private bool BeAValidGuid(Guid refreshToken)
        {
            return refreshToken != Guid.Empty;
        }
    }
}
