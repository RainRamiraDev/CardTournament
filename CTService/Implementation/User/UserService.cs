using CTDao.Interfaces.User;
using CTService.Interfaces.User;
using DataAccessApp.Dtos.Users.Admin;
using DataAccessApp.Dtos.Users.Judge;
using DataAccessApp.Dtos.Users.Organizer;
using DataAccessApp.Dtos.Users.Player;
using GameApp.Dtos.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Implementation.User
{
    public class UserService : IUserService
    {

        private readonly IUserDao _userDao;

        public UserService(IUserDao userDao)
        {
            _userDao = userDao;
        }

        //    public async Task<IEnumerable<AdminDto>> GetAllAdminsAsync()
        //    {
        //        var admins = await _userDao.GetAllAdminsAsync();
        //        var adminDtos = admins.Select(admin => new AdminDto
        //        {
        //            Username = admin.Username

        //        }).ToList();
        //        return adminDtos;
        //    }

        //    public async Task<IEnumerable<JudgeDto>> GetAllJudgesAsync()
        //    {
        //        var judges = await _userDao.GetAllJudgesAsync();
        //        var judgesDtos = judges.Select(judge => new JudgeDto
        //        {
        //            Name = judge.Name,
        //            Surname = judge.Surname,
        //            Alias = judge.Alias,
        //            Email = judge.Email,
        //            Country = judge.Country,
        //            AvatarUrl = judge.AvatarUrl

        //        }).ToList();
        //        return judgesDtos;
        //    }

        //    public async Task<IEnumerable<OrganizerDto>> GetAllOrganizersAsync()
        //    {
        //        var organizers = await _userDao.GetAllOrganizersAsync();
        //        var organizersDtos = organizers.Select(organizer => new OrganizerDto
        //        {
        //            Name = organizer.Name,
        //            Surname = organizer.Surname,
        //            Email = organizer.Email,
        //            Country = organizer.Country,

        //        }).ToList();
        //        return organizersDtos;
        //    }

        //    public async Task<IEnumerable<PlayerDto>> GetAllPlayersAsync()
        //    {
        //        var players = await _userDao.GetAllPlayersAsync();
        //        var playersDtos = players.Select(player => new PlayerDto
        //        {
        //           Name= player.Name,
        //           Surname = player.Surname,
        //           Alias = player.Alias,
        //           Country = player.Country,
        //           Email = player.Email,
        //           GamesWon = player.GamesWon,
        //           GamesLost = player.GamesLost,
        //           TournamentsWon = player.TournamentsWon,
        //           Disqualifications = player.Disqualifications,
        //           DiscualificationsReasons = player.DiscualificationsReasons,
        //           AvatarUrl = player.AvatarUrl,
        //        }).ToList();
        //        return playersDtos;
        //    }
        //}
    }
}
