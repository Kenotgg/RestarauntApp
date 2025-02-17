using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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
    /// Логика взаимодействия для DishCreationPage.xaml
    /// </summary>
    public partial class DishCreationPage : Page,INotifyPropertyChanged
    {
        const int weight = 100;
        int proteinsCount = 0;
        int fatsCount = 0;
        int carbohydratesCount = 0;
        float Kcal = 0;
        public ObservableCollection<Ingridient> addedIngridients = new ObservableCollection<Ingridient>();
        public DishCreationPage()
        {
            InitializeComponent();
            
        }

        public void LoadData() 
        {
            using (RestarauntContext db = new RestarauntContext())
            {
                DataContext = this;
                IngridentsBox.ItemsSource = db.Ingridients.ToArray();
                AddedIngridentsBox.ItemsSource = addedIngridients;
            }
        }

        private void OnAddToDish(object sender, RoutedEventArgs e)
        {
            using (RestarauntContext db = new RestarauntContext())
            {
                if (IngridentsBox.SelectedItem is Ingridient && IngridentsBox.SelectedItem != null) 
                {
                    Ingridient selectedIngridient = (Ingridient)IngridentsBox.SelectedItem;
                    addedIngridients.Add(selectedIngridient);
                    foreach (var ing in addedIngridients)
                    {
                        proteinsCount += ing.Proteins;
                        PBox.Text = proteinsCount.ToString();
                        fatsCount += ing.Fats;
                        FBox.Text = ing.Fats.ToString();
                        carbohydratesCount += ing.Carbohydrates;
                        CBox.Text = ing.Carbohydrates.ToString();
                        Kcal = (int)(proteinsCount * 4 + carbohydratesCount * 4 + fatsCount * 9)/1000;
                        KcalInputBox.Text = Kcal.ToString();
                    }
                    WeightInputBox.Text = 100.ToString();
                }
            }
                
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void OnSaveDish(object sender, RoutedEventArgs e)
        {
            using (RestarauntContext db = new RestarauntContext())
            {
                
                if(NameInputBox.Text == string.Empty) 
                {
                    MessageBox.Show("Назовите блюдо");
                    return;
                }
                if(AddedIngridentsBox.Items.Count == 0) 
                {
                    MessageBox.Show("Добавьте ингридиенты");
                    return;
                }
                var dish = new Dish { Title = NameInputBox.Text, Weight = weight, Proteins = proteinsCount, Fats = fatsCount, Carbohydrates = carbohydratesCount, KiloCalories = (int)Kcal};
                db.Dishes.Add(dish);
                db.SaveChanges();
                foreach (var item in addedIngridients)
                {
                    db.DishLists.Add(new DishList { DishId = dish.Id, IngridientId = item.Id});
                    db.SaveChanges();
                }
                MessageBox.Show("Ваше блюдо сохранено в непроверенные");
               
            }
        }

        private void OnClearAddedProducts(object sender, RoutedEventArgs e)
        {
            ClearData();
        }
        public void ClearData() 
        {
            NameInputBox.Text = null;
            addedIngridients.Clear();
            WeightInputBox.Text = null;
            carbohydratesCount = 0;
            fatsCount = 0;
            proteinsCount = 0;
            Kcal = 0;
            FBox.Text = null;
            CBox.Text = null;
            PBox.Text = null;
            KcalInputBox.Text = null;
        }

    }
}
