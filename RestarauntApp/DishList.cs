using System;
using System.Collections.Generic;

namespace RestarauntApp
{
    public partial class DishList
    {
        public int Id { get; set; }
        public int DishId { get; set; }
        public int IngridientId { get; set; }
        public int Count { get; set; }

        public virtual Dish Dish { get; set; } = null!;
        public virtual Ingridient Ingridient { get; set; } = null!;
    }
}
