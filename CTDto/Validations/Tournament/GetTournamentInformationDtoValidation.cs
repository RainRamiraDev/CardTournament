using CTDto.Tournaments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Tournament
{
    public class GetTournamentInformationDtoValidation : AbstractValidator<GetTournamentInformationDto>
    {
        public GetTournamentInformationDtoValidation()
        {
            RuleFor(x => x.Current_phase)
                .Must(value => value == 1 || value == 3).WithMessage("La fase debe ser 1 para fase de inscripcion o 3 para terminado.");
        }
    }
}
