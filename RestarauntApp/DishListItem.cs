using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestarauntApp
{
    internal class DishListItem
    {

        public int ID { get; set; }
        public string DishTitle { get; set; }
        public string IngridietnTitle { get; set; }
        public int CountOfIngridients { get; set; }
        public bool Status { get; set; }
    }
}
