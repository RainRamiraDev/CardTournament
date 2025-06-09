using CTDto.Users.Admin;
using CTDto.Users.Organizer;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Users
{
    public class AssignCardToPlayerDtoValidation : AbstractValidator<AssignCardToPlayerDto>
    {
        public AssignCardToPlayerDtoValidation()
        {
            RuleFor(x => x.id_card)
                .NotNull().WithMessage("La lista de cartas no puede ser nula.")
                .Must(list => list.Count >= 8)
                .WithMessage("Debes asignar al menos 8 cartas al jugador.");

            RuleFor(x => x.id_user)
               .NotNull().WithMessage("El id_usuario no puede ser nulo.");
        }
    }
}
