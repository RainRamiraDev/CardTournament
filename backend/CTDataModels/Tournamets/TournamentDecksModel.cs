using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Tournamets
{
    public class TournamentDecksModel
    {
        public int Id_Tournament { get; set; }
        public List<int> Id_card_series { get; set; }
        public int Id_Owner { get; set; }
    }
}
