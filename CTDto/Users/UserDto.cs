﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Users
{
    public class UserDto
    {
        public int Id_User { get; set; }
        public int IdCountry { get; set; }
        public int Id_Rol { get; set; }
        public string Fullname { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public string Passcode { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public string Avatar_Url { get; set; }

    }
}
