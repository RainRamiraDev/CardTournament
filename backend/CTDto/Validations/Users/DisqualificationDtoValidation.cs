using CTDto.Users.Admin;
using CTDto.Users.Judge;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Users
{
    public class DisqualificationDtoValidation : AbstractValidator<DisqualificationDto>
    {
        public DisqualificationDtoValidation()
        {
            RuleFor(x => x.Id_Tournament)
                .GreaterThan(0).WithMessage("El ID del torneo debe ser mayor que 0.");

            RuleFor(x => x.Id_Player)
                .GreaterThan(0).WithMessage("El ID del jugador debe ser mayor que 0.");
        }
    }
}
