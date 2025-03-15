using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace RestarauntApp
{
    /// <summary>
    /// Логика взаимодействия для EditDish.xaml
    /// </summary>
    public partial class EditDish : Page, INotifyPropertyChanged
    {
        ObservableCollection<IngridientInDish> addedIngridientsCollection = new ObservableCollection<IngridientInDish>();
        ObservableCollection<Ingridient> notAddedIngridientsCollection = new ObservableCollection<Ingridient>();
        private Dictionary<IngridientInDish, int> _ingridientWeights = new Dictionary<IngridientInDish, int>();
        int dishWeight = 0;
        public EditDish()
        {
            InitializeComponent();
        }


        public void LoadIngridients() 
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
                        var ingIndish = new IngridientInDish
                        {
                            DishID = dishList.DishId,
                            IngridientID = dishList.IngridientId,
                            Title = ing.Title,
                            Proteins = ing.Proteins,
                            Fats = ing.Fats,
                            Carbohydrates = ing.Carbohydrates,
                            Weight = dishList.Weight

                        };
                        addedIngridientsCollection.Add(ingIndish);

                    }

                }
               
                AddedProductBox.ItemsSource = addedIngridientsCollection;
                ProductBox.ItemsSource = notAddedIngridientsCollection;
            }
        }
       
        private void OnSaveChanges(object sender, RoutedEventArgs e)
        {
            using (RestarauntContext db = new RestarauntContext())
            {
                if(int.TryParse(KcalBox.Text,out int a) && int.TryParse(WeightBox.Text, out int b) && int.TryParse(ProteinsBox.Text, out int c) && int.TryParse(FatsBox.Text, out int k) && int.TryParse(CarbohydratesBox.Text, out int h))
                {
                    var findedDish = db.Dishes.FirstOrDefault(d => d.Id == PageController._dishListInfo.ID);
                    findedDish.Title = TitleBox.Text;
                    findedDish.KiloCalories = Convert.ToInt32(KcalBox.Text);
                    findedDish.Weight = Convert.ToInt32(WeightBox.Text);
                    findedDish.Proteins = Convert.ToInt32(ProteinsBox.Text);
                    findedDish.Fats = Convert.ToInt32(FatsBox.Text);
                    findedDish.Carbohydrates = Convert.ToInt32(CarbohydratesBox.Text);
                    db.SaveChanges();
                    MessageBox.Show(PageController._dishListInfo.Title + " успешно сохранено.");

                    var findedDishLists = db.DishLists.Where(d => d.Id == PageController._dishListInfo.ID);
                   
                    
                }
                
               
            }

        }
        private void UpdateAndSave()
        {
            using (RestarauntContext db = new RestarauntContext())
            {
                int? weight = 0;
                int proteins = 0;
                int fats = 0;
                int carbohydrates = 0;
                foreach (var ing in addedIngridientsCollection)
                {

                    weight += ing.Weight;
                    proteins += (int)(ing.Proteins * ing.Weight/100);
                    fats += (int)(ing.Fats * (ing.Weight / 100));
                    carbohydrates += (int)(ing.Carbohydrates * (ing.Weight / 100));
                }
                int kcal = (int)(proteins * 4 + carbohydrates * 4 + fats * 9); ;
                KcalBox.Text = kcal.ToString();
                WeightBox.Text = weight.ToString();
                ProteinsBox.Text = proteins.ToString();
                FatsBox.Text = fats.ToString();
                CarbohydratesBox.Text = carbohydrates.ToString();


                var dishToEdit = db.Dishes.FirstOrDefault(d => d.Id == PageController._dishListInfo.ID);
                dishToEdit.KiloCalories = Convert.ToInt32(KcalBox.Text);
                dishToEdit.Proteins = Convert.ToInt32(ProteinsBox.Text);
                dishToEdit.Weight = Convert.ToInt32(WeightBox.Text);
                dishToEdit.Proteins = Convert.ToInt32(ProteinsBox.Text);
                dishToEdit.Fats = Convert.ToInt32(FatsBox.Text);
                dishToEdit.Carbohydrates = Convert.ToInt32(CarbohydratesBox.Text);
                dishWeight = Convert.ToInt32(WeightBox.Text);
                db.SaveChanges();
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
                        Weight = ingridient.Weight
                    };
                    db.DishLists.Add(dishList);
                    db.SaveChanges();
                    IngridientInDish ingInDish = new IngridientInDish
                    {
                        DishID = dishList.DishId,
                        IngridientID = dishList.IngridientId,
                        Title = ingridient.Title,
                        Proteins = ingridient.Proteins,
                        Fats = ingridient.Fats,
                        Carbohydrates = ingridient.Carbohydrates,
                        Weight = dishList.Weight
                    };
                    addedIngridientsCollection.Add(ingInDish);
                    UpdateAndSave();
                    MessageBox.Show("Добавлено: " + ingridient.Title);
                }

            }


        }

        private void OnRemoveProductFromDish(object sender, RoutedEventArgs e)
        {
            if (AddedProductBox.SelectedItem is IngridientInDish selectedIngridient)
            {
                using (RestarauntContext db = new RestarauntContext())
                {
                    var productToRemove = db.DishLists.Where(d => d.DishId == PageController._dishListInfo.ID).Where(i => i.IngridientId == selectedIngridient.IngridientID).ToList();
                    foreach (var product in productToRemove)
                    {
                        db.DishLists.Remove(product);
                    }
                    db.SaveChanges();
                    addedIngridientsCollection.Remove(selectedIngridient);
                    UpdateAndSave();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        
        private void OnChangeProductWeight(object sender, RoutedEventArgs e)
        {
            using (RestarauntContext db = new RestarauntContext())
            {
                if (AddedProductBox.SelectedItem is IngridientInDish selectedIng)
                {
                    if (int.TryParse(WeightInputBox.Text, out int a))
                    {
                        IngridientInDish copiedIngridient = new IngridientInDish
                        {
                            IngridientID = selectedIng.IngridientID,
                            Title = selectedIng.Title,
                            Proteins = selectedIng.Proteins,
                            Fats = selectedIng.Fats,
                            Carbohydrates = selectedIng.Carbohydrates,
                            Weight = Convert.ToInt32(WeightInputBox.Text)

                        };
                        int ingIndex = addedIngridientsCollection.IndexOf(selectedIng);
                        addedIngridientsCollection[ingIndex] = copiedIngridient;
                        AddIngridientWeightToDictionary(copiedIngridient, int.Parse(WeightInputBox.Text));
                        if (selectedIng.Weight == 100)
                        {
                            dishWeight -= Convert.ToInt32(selectedIng.Weight.ToString());
                            dishWeight += Convert.ToInt32(GetIngridientWeight(copiedIngridient).ToString());
                            WeightBox.Text = dishWeight.ToString();

                            //MessageBox.Show(GetIngridientWeight(copiedIngridient).ToString());
                            
                        }
                       
                        else
                        {

                            dishWeight -= Convert.ToInt32(GetIngridientWeight(selectedIng).ToString());
                            //MessageBox.Show(GetIngridientWeight(selectedIng).ToString() + "убираем");
                            dishWeight += Convert.ToInt32(GetIngridientWeight(copiedIngridient).ToString());
                            //MessageBox.Show(GetIngridientWeight(copiedIngridient).ToString() + " добавляем");
                            WeightBox.Text = dishWeight.ToString();

                            //MessageBox.Show(dishWeight + " общий вес");
                            
                        }
                        var dishListToUpdate = db.DishLists.FirstOrDefault(dl => dl.IngridientId == selectedIng.IngridientID).Weight = Convert.ToInt32(WeightInputBox.Text);
                        UpdateAndSave();
                        db.SaveChanges();
                        //int tempProteins = (int)(addedIngridientsCollection[ingIndex].Proteins * addedIngridientsCollection[ingIndex].Weight / 100);
                        //int tempFats = (int)(addedIngridientsCollection[ingIndex].Fats * addedIngridientsCollection[ingIndex].Weight / 100);
                        //int tempCarbohydrates = (int)(addedIngridientsCollection[ingIndex].Carbohydrates * addedIngridientsCollection[ingIndex].Weight / 100);
                        //addedIngridientsCollection[ingIndex].Proteins = tempProteins;
                        //addedIngridientsCollection[ingIndex].Fats = tempFats;
                        //addedIngridientsCollection[ingIndex].Carbohydrates = tempCarbohydrates;
                        
                        
                    }
                    else
                    {
                        MessageBox.Show("Некорректный ввод");
                    }
                }
            }
        }

        private void AddIngridientWeightToDictionary(IngridientInDish ingridient, int weight)
        {
            if (_ingridientWeights.ContainsKey(ingridient))
            {
                _ingridientWeights[ingridient] = weight; // Обновляем вес, если ингредиент уже есть
                MessageBox.Show("Изменили");
            }
            else
            {
                _ingridientWeights.Add(ingridient, weight); // Добавляем новый ингредиент с весом
                MessageBox.Show("Добавили");
            }
        }

        // 3. Метод для получения веса ингредиента из словаря
        private int GetIngridientWeight(IngridientInDish ingridient)
        {
            if (_ingridientWeights.ContainsKey(ingridient))
            {
                return _ingridientWeights[ingridient]; // Возвращаем вес
            }
            else
            {
                return 0; // Или другое значение по умолчанию, если ингредиент не найден
            }
        }
    }
}
