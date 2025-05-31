using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Game
{
    public class RoundModel
    {
        public int Id_Tournament { get; set; }
        public int Round_Number { get; set; }
        public int Judge { get; set; }
        public bool Is_Completed { get; set; }
    }
}
