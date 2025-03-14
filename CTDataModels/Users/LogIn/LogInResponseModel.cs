using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Users.LogIn
{
    public class LogInResponseModel
    {
        public Guid RefreshToken { get; set; }
        public DateTime ExpirationDate { get; set; }

        public string AccessToken { get; set; }



    }
}
