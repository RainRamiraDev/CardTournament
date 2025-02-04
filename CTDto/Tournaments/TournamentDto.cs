using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Tournaments
{
    public class TournamentDto
    {
        public int Id_Country { get; set; }  // Nuevo campo
        public int Id_Organizer { get; set; }  // Nuevo campo
        public DateTime Start_datetime { get; set; }
        public DateTime End_datetime { get; set; }
        public int Current_Phase { get; set; }

    }
}
