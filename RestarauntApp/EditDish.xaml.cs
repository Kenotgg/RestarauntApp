using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace RestarauntApp
{
    /// <summary>
    /// Логика взаимодействия для EditDish.xaml
    /// </summary>
    public partial class EditDish : Page, INotifyPropertyChanged
    {
        ObservableCollection<Ingridient> addedIngridientsCollection = new ObservableCollection<Ingridient>();
        ObservableCollection<Ingridient> notAddedIngridientsCollection = new ObservableCollection<Ingridient>();
        public EditDish()
        {
            InitializeComponent();
        }


        public void LoadData() 
        {
            MainStackPanel.DataContext = PageController._dishListInfo;
            
            using (RestarauntContext db = new RestarauntContext())
            {
                notAddedIngridientsCollection.Clear();
                foreach (var ing in db.Ingridients.ToList())
                {
                    notAddedIngridientsCollection.Add(ing);
                }
                addedIngridientsCollection.Clear();

                var ingridentsInDish = db.DishLists.Where(dl => dl.DishId == PageController._dishListInfo.ID).ToList();
                foreach (var dishList in ingridentsInDish)
                {
                    var ing = db.Ingridients.Find(dishList.IngridientId);
                    if (ing != null) 
                    {
                        addedIngridientsCollection.Add(ing);
                    }
                    
                }
               
                AddedProductBox.ItemsSource = addedIngridientsCollection;
                ProductBox.ItemsSource = notAddedIngridientsCollection;
            }
        }
        bool canSaveChanges = false;
        private void OnSaveChanges(object sender, RoutedEventArgs e)
        {
            using (RestarauntContext db = new RestarauntContext())
            {
                if(int.TryParse(KcalBox.Text,out int a) && int.TryParse(WeightBox.Text, out int b) && int.TryParse(ProteinsBox.Text, out int c) && int.TryParse(FatsBox.Text, out int k) && int.TryParse(CarbohydratesBox.Text, out int h))
                {
                    canSaveChanges = true;
                }
                else
                {
                    canSaveChanges = false;
                }
                if (canSaveChanges) 
                {
                    var findedDish = db.Dishes.FirstOrDefault(d => d.Id == PageController._dishListInfo.ID);
                    findedDish.Title = TitleBox.Text;
                    findedDish.KiloCalories = Convert.ToInt32(KcalBox.Text);
                    findedDish.Weight = Convert.ToInt32(WeightBox.Text);
                    findedDish.Proteins = Convert.ToInt32(ProteinsBox.Text);
                    findedDish.Fats = Convert.ToInt32(FatsBox.Text);
                    findedDish.Carbohydrates = Convert.ToInt32(CarbohydratesBox.Text);
                    db.SaveChanges();
                    MessageBox.Show(PageController._dishListInfo.Title);

                    var findedDishLists = db.DishLists.Where(d => d.Id == PageController._dishListInfo.ID);
                    foreach (var d in findedDishLists)
                    {
                        MessageBox.Show(d.Ingridient.Id.ToString());
                    }
                }
                else 
                {
                    MessageBox.Show("Некорректный ввод");
                }
               
            }

        }

        private void OnAddProductToDish(object sender, RoutedEventArgs e)
        {
            if (ProductBox.SelectedItem is Ingridient ingridient) 
            {
                using (RestarauntContext db = new RestarauntContext())
                {
                    DishList dishList = new DishList 
                    {
                        DishId = PageController._dishListInfo.ID,
                        IngridientId = ingridient.Id,
                        Count = 1
                    };
                    addedIngridientsCollection.Add(ingridient);
                    db.DishLists.Add(dishList);
                    db.SaveChanges();
                    MessageBox.Show("Добавлено: " + ingridient.Title);
                }
                   
            }
                
            
        }

        private void OnRemoveProductFromDish(object sender, RoutedEventArgs e)
        {
            if (AddedProductBox.SelectedItem is Ingridient selectedIngridient)
            {
                using (RestarauntContext db = new RestarauntContext())
                {
                    var dishToRemove = db.DishLists.Where(d => d.DishId == PageController._dishListInfo.ID).Where(i => i.IngridientId == selectedIngridient.Id).ToList();
                    foreach (var dish in dishToRemove)
                    {
                        db.DishLists.Remove(dish);
                    }
                    db.SaveChanges();
                    addedIngridientsCollection.Remove(selectedIngridient);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
