using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Users.Judge
{
    public class ShowTournamentPlayersDto
    {
        public int Id_Player { get; set; }
        public string Alias { get; set; }
        public int Disqualifications { get; set; }
        public string Avatar_Url { get; set; }
    }
}
