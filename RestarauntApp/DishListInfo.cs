using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestarauntApp
{
    internal class DishListInfo
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int? Weight { get; set; } // Используйте nullable типы, если Weight, Kcal могут быть null
        public int? Kcal { get; set; }
        public bool Status { get; set; }
        public int? Proteins { get; set; }
        public int? Fats { get; set; }
        public int? Cabohydrates { get; set; }
    }
}
