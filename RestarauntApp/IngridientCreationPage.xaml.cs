using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RestarauntApp
{
    /// <summary>
    /// Логика взаимодействия для IngridientCreationPage.xaml
    /// </summary>
    public partial class IngridientCreationPage : Page, INotifyPropertyChanged
    {
        ObservableCollection<Ingridient> ingridientsCollection { get; set; } = new ObservableCollection<Ingridient>();
        public IngridientCreationPage()
        {
            InitializeComponent();
            DataContext = this;
            using (RestarauntContext db = new RestarauntContext())
            {
                foreach (Ingridient i in db.Ingridients.ToArray()) 
                {
                    
                    ingridientsCollection.Add(i);
                }
                ProductsBox.ItemsSource = ingridientsCollection;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        
        private void OnSaveProduct(object sender, RoutedEventArgs e)
        {
            using (RestarauntContext db = new RestarauntContext())
            {
                bool canAdd = false;
                if (int.TryParse(ProteinBox.Text,out int a) && int.TryParse(FatsBox.Text, out int b) && int.TryParse(CarbohydratesBox.Text, out int c)) 
                {
                    canAdd = true;
                }
                if (canAdd)
                {
                    db.Ingridients.Add(new Ingridient { Title = ProductNameBox.Text, Proteins = Convert.ToInt32(ProteinBox.Text), Fats = Convert.ToInt32(FatsBox.Text), Carbohydrates = Convert.ToInt32(CarbohydratesBox.Text), Weight = 100 });
                    MessageBox.Show("Продукт был добавлен");
                }
                else 
                {
                    MessageBox.Show("Некорректный ввод");
                }
                db.SaveChanges();
                ingridientsCollection.Clear();
                foreach (Ingridient i in db.Ingridients.ToArray())
                {

                    ingridientsCollection.Add(i);
                }
                db.SaveChanges();
                
            }
        }

        private void OnDeleteProduct(object sender, RoutedEventArgs e)
        {
            
            using (RestarauntContext db = new RestarauntContext())
            {
                if(ProductsBox.SelectedItem is Ingridient selectedItem)   
                {
                    int selectedIndex = selectedItem.Id;
                    Ingridient? ingridientToDelete = db.Ingridients.FirstOrDefault(i => i.Id == selectedIndex);
                    DishList? dishList = db.DishLists.FirstOrDefault(i => i.IngridientId == ingridientToDelete.Id);
                    if (dishList == null) 
                    {
                        ingridientsCollection.RemoveAt(ProductsBox.SelectedIndex);
                        db.Ingridients.Remove(ingridientToDelete);
                        db.SaveChanges();
                    }
                    else 
                    {
                        MessageBox.Show("Продукт используется в блюде");
                    }
                   
                }
               
            }
        }
        public void ClearData() 
        {
            ProductNameBox.Text = string.Empty;
            ProteinBox.Text = string.Empty;
            FatsBox.Text = string.Empty;
            CarbohydratesBox.Text = string.Empty;
        }
    }
}
