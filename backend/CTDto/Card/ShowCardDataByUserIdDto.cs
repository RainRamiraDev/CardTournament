using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Card
{
    public class ShowCardDataByUserIdDto
    {
        public string Illustration { get; set; }
        public int Attack { get; set; }

        public int Defense { get; set; }

        public string Series_Name { get; set; }

        public string Release_Date { get; set; }
    }
}
