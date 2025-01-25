using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Game
{
    public class GamePlayersModel
    {
        public int Id_Game { get; set; }
        public List<int> Id_Player { get; set; }
        public bool Is_Winner { get; set; }
    }
}
