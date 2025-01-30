using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CTDataModels.Card
{
    public class ShowCardsModel
    {
        public string Series_name { get; set; }
        public string Illustration { get; set; }
        public int Attack { get; set; }
        public int Deffense { get; set; }
        public DateTime Release_Date { get; set; }

    }
}
