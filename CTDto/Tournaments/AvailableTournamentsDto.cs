﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Tournaments
{
    public class TournamentsInformationDto
    {
        public int Id_Torneo { get; set; }
        public string Pais { get; set; }
        public DateTime FechaDeInicio { get; set; }
        public DateTime FechaDeFinalizacion { get; set; }
        public string Jueces { get; set; }
        public string Series { get; set; }
        public string Jugadores { get; set; }
        public string Ganador { get; set; }

    }
}
