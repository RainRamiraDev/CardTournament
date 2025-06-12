using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Users.Organizer
{
    public class AssignCardToPlayerDto
    {
        public int id_user { get; set; }
        public List<int> id_card { get; set; }
    }
}
