using CTDto.Tournaments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Tournament
{
    public class TournamentRequestToResolveDtoValidation :  AbstractValidator<TournamentRequestToResolveDto>
    {
        public TournamentRequestToResolveDtoValidation()
        {
            RuleFor(x => x.Tournament_Id)
                .GreaterThan(0).WithMessage("El Tournament_Id debe ser mayor que 0.");
        }
    }
}
