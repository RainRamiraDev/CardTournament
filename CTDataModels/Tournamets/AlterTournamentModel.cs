using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Tournamets
{
    public class AlterTournamentModel
    {
        public int Id_tournament { get; set; }
        public int Id_Country { get; set; }
        public int Id_Organizer { get; set; }
        public DateTime Start_datetime { get; set; }
        public DateTime End_datetime { get; set; }
        public List<int> Judges_Id { get; set; }
        public List<int> Series_Id { get; set; }
    }
}
