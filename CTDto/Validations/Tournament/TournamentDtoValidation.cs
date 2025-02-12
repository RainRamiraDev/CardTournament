using CTDto.Tournaments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Tournament
{
    public class TournamentDtoValidation : AbstractValidator<TournamentDto>
    {
        public TournamentDtoValidation()
        {
            RuleFor(t => t.Id_Country)
            .GreaterThan(0).WithMessage("El Id_Country debe ser un número positivo.");

            RuleFor(t => t.Id_Organizer)
                .GreaterThan(0).WithMessage("El Id_Organizer debe ser un número positivo.");

            RuleFor(t => t.Start_datetime)
                .GreaterThan(DateTime.UtcNow).WithMessage("La fecha de inicio debe ser en el futuro.");

            //RuleFor(t => t.Current_Phase)
            //    .GreaterThanOrEqualTo(1).WithMessage("La fase actual debe ser al menos 1.");

            RuleFor(t => t.Judges)
                .NotEmpty().WithMessage("Debe haber al menos un juez asignado.")
                .Must(judges => judges.Count == 3)
                .WithMessage("Debe haber exactamente 3 jueces asignados.");

            RuleFor(t => t.Series_name)
                .NotEmpty().WithMessage("Debe haber al menos una serie asignada.")
                .Must(series => series.Count >= 1)
                .WithMessage("Debe haber al menos una serie definida.");
        }
    }
}
