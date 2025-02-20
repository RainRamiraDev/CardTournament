using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Users
{
    public class UserCreationModel
    {
        public int Id_User { get; set; }
        public int Id_Country { get; set; }
        public int Id_Rol { get; set; }
        public string Passcode { get; set; }
        public string Fullname { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public int Games_Won { get; set; }
        public int Games_Lost { get; set; }
        public int Disqualifications { get; set; }
        public int Ki { get; set; }
        public string Avatar_Url { get; set; }
    }
}
