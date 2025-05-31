using CTDto.Tournaments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Tournament
{
    public class AlterTournamentDtoValidation : AbstractValidator<AlterTournamentDto>
    {
        public AlterTournamentDtoValidation()
        {
            RuleFor(x => x.Id_tournament)
           .GreaterThan(0).WithMessage("El ID del torneo debe ser mayor que 0.");

            RuleFor(x => x.Id_Country)
                .GreaterThan(0).WithMessage("El ID del país debe ser mayor que 0.");

            RuleFor(x => x.Start_datetime)
                      .LessThan(x => x.End_datetime).WithMessage("La fecha de inicio debe ser anterior a la fecha de finalización.")
                      .Must(BeInUtc).WithMessage("La fecha de inicio debe estar en formato UTC.");

            RuleFor(x => x.End_datetime)
                .GreaterThan(x => x.Start_datetime).WithMessage("La fecha de finalización debe ser posterior a la fecha de inicio.")
                .Must(BeInUtc).WithMessage("La fecha de finalización debe estar en formato UTC.");

            RuleFor(x => x.Judges_Id)
                .NotEmpty().WithMessage("La lista de jueces no puede estar vacía.")
                .Must(judges => judges.All(id => id > 0)).WithMessage("Todos los IDs de los jueces deben ser mayores que 0.");

            RuleFor(x => x.Series_Id)
                .NotEmpty().WithMessage("La lista de series no puede estar vacía.")
                .Must(series => series.All(id => id > 0)).WithMessage("Todos los IDs de las series deben ser mayores que 0.");
        }

        private bool BeInUtc(DateTime date)
        {
            return date.Kind == DateTimeKind.Utc;
        }
    }
}
