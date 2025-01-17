using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Users
{
    public class UserModel
    {
        public int Id_User { get; set; }
        public string Country { get; set; }
        public int IdRole { get; set; }

        public string Passcode { get; set; }
        public string Fullname { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int Disqualifications { get; set; }
        public string Avatar_Url { get; set; }
    }
}
