﻿using CTDataModels.Tournamets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDao.Interfaces.Game
{
    public interface IGameDao
    {
        Task<int> CreateGameAsync(GameModel game);

    }
}
