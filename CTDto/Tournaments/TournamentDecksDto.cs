using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Card
{
    public class TournamentDecksDto
    {
        public int Id_Tournament { get; set; }
        public List<string> illustration { get; set; }
        public int Id_Owner { get; set; }
    }
}
