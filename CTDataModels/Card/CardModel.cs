using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessApp.Models.Card
{
    public class CardModel
    {
        public int Id_card { get; set; }
        public string Illustration { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }

    }
}
