using CTDto.Tournaments;
using CTDto.Users.LogIn;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Users.Tournament
{
    public class TournamentJudgeDtoValidation : AbstractValidator<TournamentJudgeDto>
    {
        public TournamentJudgeDtoValidation()
        {
            RuleFor(t => t.Id_Tournament)
                .GreaterThan(0).WithMessage("El Id_Tournament debe ser un número positivo.");

            RuleFor(t => t.Judges)
                .NotEmpty().WithMessage("Debe haber al menos un juez en la lista.")
                .Must(judges => judges.All(j => !string.IsNullOrWhiteSpace(j)))
                .WithMessage("Los alias de los jueces no pueden estar vacíos.")
                .ForEach(judge =>
                {
                    judge.MaximumLength(50).WithMessage("El alias de un juez no puede superar los 50 caracteres.");
                });
        }


    }
}
