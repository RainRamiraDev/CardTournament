﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Users.LogIn
{
    public class UserRequestDto
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Passcode { get; set; }
        public int Id_Rol { get; set; }
    }
}
