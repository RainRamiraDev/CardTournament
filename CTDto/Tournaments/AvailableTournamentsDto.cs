using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Tournaments
{
    public class AvailableTournamentsDto
    {
        public DateTime Start_DateTime { get; set; }
        public DateTime End_DateTime { get; set; }
        public string Tournament_Country { get; set; }
        public string Organizer_Alias { get; set; }
        public string Judges { get; set; }
        public string Series_Played { get; set; }
        public string Players { get; set; }
        public string Disqualified_Players { get; set; }
        public int Total_Games { get; set; }
        public int Total_Rounds { get; set; }

    }
}
