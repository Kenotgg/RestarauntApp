using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
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
       
       
        private Dictionary<IngridientInDish, int> _ingridientWeights = new Dictionary<IngridientInDish, int>();
        private ObservableCollection<IngridientInDish> addedIngridients = new ObservableCollection<IngridientInDish>();
        public List<DishList> dishListsToAdd = new List<DishList>();
        private DishList selectedIngridient = new DishList();
        int weight = 0;
        int proteinsCount = 0;
        int fatsCount = 0;
        int carbohydratesCount = 0;
        float Kcal = 0;
        public DishCreationPage()
        {
            InitializeComponent();
        }

        public void LoadData() 
        {
            using (RestarauntContext db = new RestarauntContext())
            {
                DataContext = this;
                AllIngridentsBox.ItemsSource = db.Ingridients.ToArray();
                AddedIngridentsBox.ItemsSource = addedIngridients;
            }
        }
        private void UpdateInfo() 
        {
            WeightInputBox.DataContext = selectedIngridient;
        }
        private void OnAddToDish(object sender, RoutedEventArgs e)
        {
            using (RestarauntContext db = new RestarauntContext())
            {
                if (AllIngridentsBox.SelectedItem is Ingridient selIng && AllIngridentsBox.SelectedItem != null) 
                {
                    Ingridient selectedIngridient = (Ingridient)AllIngridentsBox.SelectedItem;
                    IngridientInDish addedIngridient = new IngridientInDish
                    {
                        IngridientID = selectedIngridient.Id,
                        Title = selectedIngridient.Title,
                        Proteins = selectedIngridient.Proteins,
                        Weight = selectedIngridient.Weight,
                        Carbohydrates = selectedIngridient.Carbohydrates,
                        Fats = selectedIngridient.Fats
                    };
                    addedIngridients.Add(addedIngridient);
                    proteinsCount += (int)(addedIngridient.Proteins * addedIngridient.Weight / 100);
                    PBox.Text = proteinsCount.ToString();
                    fatsCount += (int)(addedIngridient.Fats * addedIngridient.Weight/100);
                    FBox.Text = fatsCount.ToString();
                    carbohydratesCount += (int)(addedIngridient.Carbohydrates * addedIngridient.Weight / 100);
                    CBox.Text = carbohydratesCount.ToString();
                    Kcal = (int)(proteinsCount * 4 + carbohydratesCount * 4 + fatsCount * 9);
                    KcalShowBox.Text = Kcal.ToString();
                    if (selectedIngridient.Weight != null)
                    {
                        weight += int.Parse(addedIngridient.Weight.ToString());
                    }
                    else
                    {
                        weight += GetIngridientWeight(addedIngridient);
                    }
                    WeightShowBox.Text = weight.ToString();
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
                
                var selectByDishId = db.DishLists.Where(d => d.Id == dish.Id);
                db.DishLists.RemoveRange(selectByDishId);
                db.SaveChanges();
                db.Dishes.Add(dish);
                db.SaveChanges();
                foreach (var ing in addedIngridients)
                {
                    if (ing.Weight == null) 
                    {
                        db.DishLists.Add(new DishList { DishId = dish.Id, IngridientId = ing.IngridientID, Weight = GetIngridientWeight(ing) });
                        db.SaveChanges();
                    }
                    else 
                    {
                        db.DishLists.Add(new DishList { DishId = dish.Id, IngridientId = ing.IngridientID, Weight = ing.Weight });
                        db.SaveChanges();
                    }

                    
                   
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
            WeightShowBox.Text = null;
            carbohydratesCount = 0;
            fatsCount = 0;
            proteinsCount = 0;
            Kcal = 0;
            FBox.Text = null;
            CBox.Text = null;
            PBox.Text = null;
            KcalShowBox.Text = null;
            weight = 0;
        }

        public void ClearTextAndValues() 
        {
            carbohydratesCount = 0;
            fatsCount = 0;
            proteinsCount = 0;
            Kcal = 0;
            weight = 0;
            NameInputBox.Text = null;
            WeightShowBox.Text = null;
            FBox.Text = null;
            CBox.Text = null;
            PBox.Text = null;
            KcalShowBox.Text = null;
           
        }

        
        private void AddedIngridentsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // = AddedIngridentsBox.SelectedItem as Ingridient;
            if (AddedIngridentsBox.SelectedItem is Ingridient selectedIng) 
            {
                //selectedIngridient = selectedIng;
                UpdateInfo();   
                
            }
            
        }
        int tempWeight = 0;
        private void OnSaveWeight(object sender, RoutedEventArgs e)
        {
            using (RestarauntContext db = new RestarauntContext())
            {
                if (AddedIngridentsBox.SelectedItem is IngridientInDish selectedIng)
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
                        int ingIndex = addedIngridients.IndexOf(selectedIng);
                        addedIngridients[ingIndex] = copiedIngridient;
                        AddIngridientWeightToDictionary(copiedIngridient, int.Parse(WeightInputBox.Text));
                        if (selectedIng.Weight == 100)
                        {
                            weight -= Convert.ToInt32(selectedIng.Weight.ToString());
                            weight += Convert.ToInt32(GetIngridientWeight(copiedIngridient).ToString());
                            WeightShowBox.Text = weight.ToString();
                            
                            MessageBox.Show(GetIngridientWeight(copiedIngridient).ToString());
                        }
                        else
                        {                     
                           
                            weight -= Convert.ToInt32(GetIngridientWeight(selectedIng).ToString());
                            MessageBox.Show(GetIngridientWeight(selectedIng).ToString() + "убираем");
                            weight += Convert.ToInt32(GetIngridientWeight(copiedIngridient).ToString());
                            MessageBox.Show(GetIngridientWeight(copiedIngridient).ToString() + " добавляем");
                            WeightShowBox.Text = weight.ToString();
                            
                            MessageBox.Show(weight + " общий вес");
                          
                        }
                    }
                    else
                    {
                        MessageBox.Show("Некорректный ввод");
                    }
                }
            }
        }
        private void AddIngridientWeightToDictionary(IngridientInDish ingridient,int weight) 
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
