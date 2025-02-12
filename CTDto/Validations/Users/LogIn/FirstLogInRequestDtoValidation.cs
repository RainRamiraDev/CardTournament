using CTDto.Users.LogIn;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Users.LogIn
{
    public class FirstLogInRequestDtoValidation : AbstractValidator<LoginRequestDto>
    {
        public FirstLogInRequestDtoValidation()
        {
          
            RuleFor(x => x.Fullname)
                .NotEmpty().WithMessage("El nombre completo no puede estar vacío.")
                .MaximumLength(50).WithMessage("El nombre completo no puede superar los 50 caracteres.");

           
            RuleFor(x => x.Id_Rol)
                .InclusiveBetween(1, 4).WithMessage("El Id_Rol debe estar entre 1 y 4.");

         
            RuleFor(x => x.Passcode)
                .NotEmpty().WithMessage("La contraseña no puede estar vacía.")
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$")
                .WithMessage("La contraseña debe tener al menos 8 caracteres, incluyendo letras y números.");
        }
    }
 }

