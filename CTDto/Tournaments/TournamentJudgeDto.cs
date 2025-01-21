using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Tournaments
{
    public class TournamentJudgeDto
    {
        public int Id_Tournament { get; set; }
        public List<string> Judges { get; set; } // Lista de alias de los jueces
    }
}
