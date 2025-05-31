using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Users.Judge
{
    public class ShowTournamentPlayersModel
    {
        public int Id_Player { get; set; }
        public string Alias { get; set; }
        public int Disqualifications { get; set; }
        public string Avatar_Url { get; set; }
    }
}
