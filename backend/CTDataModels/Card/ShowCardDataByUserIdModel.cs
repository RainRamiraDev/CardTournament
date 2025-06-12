using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDataModels.Card
{
    public class ShowCardDataByUserIdModel
    {
        public string Illustration { get; set; }
        public int Attack { get; set; }

        public int Defense { get; set; }

        public string Series_Name { get; set; }

        public string Release_Date { get; set; }
    }
}
