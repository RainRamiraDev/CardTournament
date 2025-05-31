using CTDto.Users;
using CTDto.Users.Admin;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Users
{
    public class AlterUserDtoValidation : AbstractValidator<AlterUserDto>
    {
        public AlterUserDtoValidation()
        {
            RuleFor(user => user.Id_User)
            .GreaterThan(0).WithMessage("El Id_Country debe ser un valor positivo.");

            RuleFor(user => user.New_IdCountry)
            .GreaterThan(0).WithMessage("El Id_Country debe ser un valor positivo.");

            RuleFor(user => user.New_Id_Rol)
                .GreaterThan(0).WithMessage("El Id_Rol debe ser un valor positivo.");

            RuleFor(user => user.New_Fullname)
                .NotEmpty().WithMessage("El nombre completo es requerido.")
                .Length(2, 100).WithMessage("El nombre completo debe tener entre 2 y 50 caracteres.");

            RuleFor(user => user.New_Alias)
                .NotEmpty().WithMessage("El alias es requerido.")
                .Length(3, 50).WithMessage("El alias debe tener entre 3 y 20 caracteres.");

            RuleFor(user => user.New_Email)
                .NotEmpty().WithMessage("El email es requerido.")
                .EmailAddress().WithMessage("El email no es válido.");

            RuleFor(user => user.New_Avatar_Url)
                .NotEmpty().WithMessage("La URL del avatar es requerida.")
                .Must(BeAValidUrl).WithMessage("La URL del avatar no es válida.");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

    }
}
