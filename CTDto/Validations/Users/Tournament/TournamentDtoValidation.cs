using CTDto.Tournaments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Users.Tournament
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
                .LessThan(t => t.End_datetime).WithMessage("La fecha de inicio debe ser anterior a la fecha de fin.");

            RuleFor(t => t.End_datetime)
                .GreaterThan(t => t.Start_datetime).WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.");

            RuleFor(t => t.Current_Phase)
                .NotEmpty().WithMessage("La fase actual no puede estar vacía.");
              
        }
    }
}
