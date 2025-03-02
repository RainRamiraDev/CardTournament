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

            // Regla para limitar el margen de fechas
            RuleFor(t => t.Start_datetime)
                .Must(start => start < DateTime.UtcNow.AddDays(365)) // No debe ser más de un año en el futuro
                .WithMessage("La fecha de inicio no puede ser más de un año en el futuro.");

            RuleFor(t => t.End_datetime)
                .GreaterThan(DateTime.UtcNow).WithMessage("La fecha de fin debe ser en el futuro.");

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
    }
}
