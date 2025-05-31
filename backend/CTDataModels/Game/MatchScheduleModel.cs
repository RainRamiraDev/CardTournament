using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Game
{
    public class MatchScheduleModel
    {
        public DateTime Match_Date { get; set; } // Fecha y hora del partido
        public int Id_Match { get; set; } // Número de partido
    }
}
