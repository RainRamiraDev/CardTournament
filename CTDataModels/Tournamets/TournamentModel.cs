using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Tournamets
{
    public class TournamentModel
    {
        public int id_tournament { get; set; }
        public string Country { get; set; }
        public string Organizer { get; set; }
        public DateTime Start_datetime { get; set; }
        public DateTime End_datetime { get; set; }
        public string Current_Phase { get; set; }
    }
}
