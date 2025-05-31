using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Users.Judge
{
    public class DisqualificationModel
    {
        public int Id_Tournament { get; set; }

        public int Id_Player { get; set; }

        public int Id_Judge { get; set; }


    }
}
