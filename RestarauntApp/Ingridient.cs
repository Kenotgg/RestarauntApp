using System;
using System.Collections.Generic;

namespace RestarauntApp
{
    public partial class Ingridient
    {
        public Ingridient()
        {
            DishLists = new HashSet<DishList>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int Proteins { get; set; }
        public int Fats { get; set; }
        public int Carbohydrates { get; set; }

        public virtual ICollection<DishList> DishLists { get; set; }
    }
}
