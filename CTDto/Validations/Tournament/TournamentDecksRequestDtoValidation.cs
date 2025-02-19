using CTDto.Card;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Tournament
{
    public class TournamentDecksRequestDtoValidation : AbstractValidator<TournamentDecksDto>
    {
        public TournamentDecksRequestDtoValidation()
        {
            RuleFor(x => x.Id_Tournament)
                .GreaterThan(0).WithMessage("El Id_Tournament debe ser mayor que 0.");

            RuleFor(x => x.Cards)
                .NotNull().WithMessage("La lista de ilustraciones no puede ser nula.")
                .Must(x => x.Count == 15).WithMessage("Debe haber exactamente 15 ilustraciones por Id_Owner.");

            RuleFor(x => x.Id_Owner)
                .GreaterThan(0).WithMessage("El Id_Owner debe ser mayor que 0.");
        }
    }
}
