using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Tournaments
{
    public class TournamentDto
    {
        public int Id_Country { get; set; }
        public int Id_Organizer { get; set; }
        public DateTime Start_datetime { get; set; }
        public DateTime End_datetime { get; set; }
        public List<string> Judges_Alias { get; set; }
        public List<string> Series_name { get; set; }
    }
}
