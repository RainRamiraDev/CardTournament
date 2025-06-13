using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Users.Admin
{
    public class GetUserByIdDto
    {
        public int Id_Country { get; set; }
        public int Id_Rol { get; set; }
        public string Fullname { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public string Avatar_Url { get; set; }
    }
}
