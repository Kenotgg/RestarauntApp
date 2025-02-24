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
        int weight = 0;
        int proteinsCount = 0;
        int fatsCount = 0;
        int carbohydratesCount = 0;
        float Kcal = 0;
        DishList selectedIngridient = new DishList();
        private Dictionary<Ingridient, int> _ingridientWeights = new Dictionary<Ingridient, int>();
        public ObservableCollection<Ingridient> addedIngridients = new ObservableCollection<Ingridient>();
        public List<DishList> dishListsToAdd = new List<DishList>();
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
        private void UpdateInfo() 
        {
            WeightInputBox.DataContext = selectedIngridient;
        }
        private void OnAddToDish(object sender, RoutedEventArgs e)
        {
            using (RestarauntContext db = new RestarauntContext())
            {
                if (IngridentsBox.SelectedItem is Ingridient selIng && IngridentsBox.SelectedItem != null) 
                {
                    
                    Ingridient selectedIngridient = (Ingridient)IngridentsBox.SelectedItem;
                    addedIngridients.Add(selectedIngridient);
                    foreach (var ing in addedIngridients)
                    {
                        proteinsCount += ing.Proteins;
                        PBox.Text = proteinsCount.ToString();
                        fatsCount += ing.Fats;
                        FBox.Text = fatsCount.ToString();
                        carbohydratesCount += ing.Carbohydrates;
                        CBox.Text = carbohydratesCount.ToString();
                        Kcal = (int)(proteinsCount * 4 + carbohydratesCount * 4 + fatsCount * 9)/1000;
                        KcalShowBox.Text = Kcal.ToString();
                        if(ing.Weight != null) 
                        {
                            weight += int.Parse(ing.Weight.ToString());
                            //MessageBox.Show(ing.Weight.ToString());
                        }
                        else 
                        {
                            weight += GetIngridientWeight(ing);
                        }
                        
                        WeightShowBox.Text = weight.ToString();
                    }
                    
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
                foreach (var ing in addedIngridients)
                {
                    if (ing.Weight == null) 
                    {
                        db.DishLists.Add(new DishList { DishId = dish.Id, IngridientId = ing.Id, Weight = GetIngridientWeight(ing) });
                        db.SaveChanges();
                    }
                    else 
                    {
                        db.DishLists.Add(new DishList { DishId = dish.Id, IngridientId = ing.Id, Weight = ing.Weight });
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

        
        private void AddedIngridentsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // = AddedIngridentsBox.SelectedItem as Ingridient;
            if (AddedIngridentsBox.SelectedItem is Ingridient selectedIng) 
            {
                //selectedIngridient = selectedIng;
                UpdateInfo();   
                
            }
            
        }

        private void OnSaveWeight(object sender, RoutedEventArgs e)
        {
            if (AddedIngridentsBox.SelectedItem is Ingridient selectedIng)
            {
                if(int.TryParse(WeightInputBox.Text, out int a))
                {
                    Ingridient copiedIngridient = new Ingridient 
                    {
                        Id = selectedIng.Id,
                        Title = selectedIng.Title,
                        Proteins = selectedIng.Proteins,
                        Fats = selectedIng.Fats,
                        Carbohydrates = selectedIng.Carbohydrates
                    };
                    int ingIndex = addedIngridients.IndexOf(selectedIng);
                    addedIngridients[ingIndex] = copiedIngridient;
                    //MessageBox.Show(copiedIngridient.Title.ToString());
                    AddIngridientWeightToDictionary(copiedIngridient, int.Parse(WeightInputBox.Text));
                    if (weight != null) 
                    {
                        weight -= Convert.ToInt32(selectedIng.Weight.ToString());
                        weight += Convert.ToInt32(GetIngridientWeight(copiedIngridient).ToString());
                        WeightShowBox.Text = weight.ToString();
                        MessageBox.Show(GetIngridientWeight(copiedIngridient).ToString());
                    }
                   
                }
                else 
                {
                    MessageBox.Show("Некорректный ввод");
                }
                
                //foreach (var item in addedIngridients)
                //{
                //    GetIngridientWeight(item);
                //}
                    
            }
        }
        private void AddIngridientWeightToDictionary(Ingridient ingridient,int weight) 
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
        private int GetIngridientWeight(Ingridient ingridient)
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
