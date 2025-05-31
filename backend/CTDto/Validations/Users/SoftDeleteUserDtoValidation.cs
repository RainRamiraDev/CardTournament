using CTDto.Users.Admin;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Users
{
    public class SoftDeleteUserDtoValidation : AbstractValidator<SoftDeleteUserDto>
    {
        public SoftDeleteUserDtoValidation()
        {
            RuleFor(x => x.Id_User)
                .GreaterThan(0).WithMessage("El ID de usuario debe ser mayor que 0.");
        }
    }
}
