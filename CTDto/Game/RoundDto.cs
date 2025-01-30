using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Game
{
    public class RoundDto
    {
        public int Id_Tournament { get; set; }
        public int Round_Number { get; set; }
        public bool Is_Completed { get; set; }
    }
}
