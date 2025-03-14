using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Game
{
    public class MatchDto
    {
        public int Id_Round { get; set; }
        public int Id_Player1 { get; set; }
        public int Id_Player2 { get; set; }
        public int Winner { get; set; }
        public DateTime Match_Date { get; set; }
    }
}
