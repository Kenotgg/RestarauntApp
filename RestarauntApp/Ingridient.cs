using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestarauntApp
{
    public partial class Ingridient : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
