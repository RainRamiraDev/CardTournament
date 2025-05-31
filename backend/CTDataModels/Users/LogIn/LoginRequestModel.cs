using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Users.LogIn
{
    public class LoginRequestModel
    {
        public string Fullname { get; set; }
        public string Passcode { get; set; }
        public int Id_Rol { get; set; }
    }
}
