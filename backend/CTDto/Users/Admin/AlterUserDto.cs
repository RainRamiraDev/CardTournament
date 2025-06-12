using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Users.Admin
{
    public class AlterUserDto
    {
        public int Id_User { get; set; }
        public int New_IdCountry { get; set; }
        public int New_Id_Rol { get; set; }
        public string New_Fullname { get; set; }
        public string New_Alias { get; set; }
        public string New_Email { get; set; }
        public string New_Avatar_Url { get; set; }
    }
}
