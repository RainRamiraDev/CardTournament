﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Tournamets
{
    public class TournamentRequestToResolveModel
    {
        public int Tournament_Id { get; set; }
        public int availableHoursPerDay { get; set; }
    }
}
