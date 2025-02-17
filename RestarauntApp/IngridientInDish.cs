using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestarauntApp
{
    internal class IngridientInDish
    {
        public int DishID { get; set; }
        public int IngridientID { get; set;}
        public string IngridientName { get; set;}
        public int Proteins { get; set;}
        public int Fats { get; set;}
        public int Carbohydrates { get; set;}
    }
}
