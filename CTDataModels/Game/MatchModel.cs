using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Game
{
    public class MatchModel
    {
        public int Id_Round { get; set; }
        public int Id_Game { get; set; }
        public int Id_Player1 { get; set; }
        public int Id_Player2 { get; set; }
        public int Winner { get; set; }
    }
}
