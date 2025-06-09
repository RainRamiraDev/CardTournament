using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Users
{
    public class AssignCardToPlayerModel
    {
        public int id_user { get; set; }
        public List<int> id_card { get; set; }
    }
}
