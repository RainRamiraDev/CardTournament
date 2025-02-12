﻿using CTDataModels.Users;
using CTDataModels.Users.LogIn;
using CTDataModels.Users.Organizer;
using CTDto.Users.Organizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDao.Interfaces.User
{
    public interface IUserDao
    {
        //log In
        Task<int> CreateWhitHashedPasswordAsync(LoginRequestModel user);
        Task<UserModel> GetUserWhitTokenAsync(int id);
        Task<UserModel> LogInAsync(string fullname);

        //Judge
        Task<IEnumerable<UserModel>> GetAllJudgeAsync();

        //Player
        Task<int> GetPlayerKiByIdAsync(int playerIds);

        Task<IEnumerable<CountriesListModel>> GetAllCountriesAsync();


    }
}
