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

            RuleFor(t => t.Start_datetime)
                .GreaterThan(DateTime.UtcNow).WithMessage("La fecha de inicio debe ser en el futuro.");

            RuleFor(x => x.Start_datetime)
             .LessThan(x => x.End_datetime).WithMessage("La fecha de inicio debe ser anterior a la fecha de finalización.")
             .Must(BeInUtc).WithMessage("La fecha de inicio debe estar en formato UTC.");

            RuleFor(x => x.End_datetime)
                .GreaterThan(x => x.Start_datetime).WithMessage("La fecha de finalización debe ser posterior a la fecha de inicio.")
                .Must(BeInUtc).WithMessage("La fecha de finalización debe estar en formato UTC.");

            RuleFor(t => t.End_datetime)
                .GreaterThan(t => t.Start_datetime)
                .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.");

            // Regla para la duración mínima del torneo
            RuleFor(t => t)
                .Must(t => t.Start_datetime.AddDays(2) < t.End_datetime) // La duración mínima debe ser de 2 días
                .WithMessage("La duración del torneo debe ser de al menos 2 días.");

            RuleFor(t => t.Judges_Id)
                .NotEmpty().WithMessage("Debe haber al menos un juez asignado.")
                .Must(judges => judges.Count == 3)
                .WithMessage("Debe haber exactamente 3 jueces asignados.");

            RuleFor(t => t.Series_Id)
                .NotEmpty().WithMessage("Debe haber al menos una serie asignada.")
                .Must(series => series.Count >= 1)
                .WithMessage("Debe haber al menos una serie definida.");
        }

        private bool BeInUtc(DateTime date)
        {
            return date.Kind == DateTimeKind.Utc;
        }
    }
}
