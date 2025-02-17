using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page, INotifyPropertyChanged
    {
        //List<ListBox> listBoxes = new List<ListBox>();

        private ObservableCollection<DishListInfo> aprovedDishesCollection { get; set; } = new ObservableCollection<DishListInfo>();
        private ObservableCollection<DishListInfo> notAprovedDishesCollection { get; set; } = new ObservableCollection<DishListInfo>();

        //Я делаю перемещение блюд между листами, и логично сделать это за счет смены статуса, и обновления после этого страницы, также есть вариант, работать немного отдельно и просто прокидывать отображение за счет того, что каждый лист бокс привязан к своей ObservableCollection, ID конечно меняется, просто нет нужды обновлять страницу по нему
        public MainPage()
        {
            InitializeComponent();
            GetData();
        }

        void GetData()
        {
            using (RestarauntContext db = new RestarauntContext())
            {

                //Выборка для списка блюд 
                var dishList = from dish in db.Dishes
                               join singleDishList in db.DishLists on dish.Id equals singleDishList.DishId
                               join ingridient in db.Ingridients on singleDishList.IngridientId equals ingridient.Id
                               select new DishListItem
                               {
                                   ID = singleDishList.Id,
                                   DishTitle = dish.Title,
                                   IngridietnTitle = ingridient.Title,
                                   CountOfIngridients = singleDishList.Count,
                                   Status = dish.Status
                               };

                var dishes = db.Dishes.Select(p => new DishListInfo
                {
                    ID = p.Id,
                    Title = p.Title,
                    Weight = p.Weight,
                    Kcal = p.KiloCalories,
                    Status = p.Status,
                    Proteins = p.Proteins,
                    Fats = p.Fats,
                    Cabohydrates = p.Carbohydrates
                }
                );

                foreach (var dish in dishes.ToArray().Where(d => d.Status == true))
                {
                    aprovedDishesCollection.Add(dish);
                }
                foreach (var dish in dishes.ToArray().Where(d => d.Status == false))
                {
                    notAprovedDishesCollection.Add(dish);
                }
                AprovedDishesListBox.ItemsSource = aprovedDishesCollection;
                NotAprovedDishesListBox.ItemsSource = notAprovedDishesCollection;

            }



        }

        private void OnRightButton(object sender, RoutedEventArgs e)
        {

            using (RestarauntContext db = new RestarauntContext())
            {

                if (AprovedDishesListBox.SelectedItems.Count == 1)
                {
                    DishListInfo dishToMove = (DishListInfo)AprovedDishesListBox.Items[AprovedDishesListBox.SelectedIndex];

                    aprovedDishesCollection.RemoveAt(AprovedDishesListBox.SelectedIndex);
                    var dishToChange = db.Dishes.FirstOrDefault(d => d.Id == dishToMove.ID);
                    dishToChange.Status = false;
                    db.SaveChanges();
                    notAprovedDishesCollection.Add(dishToMove);

                }
            }
        }

        private void OnLeftButton(object sender, RoutedEventArgs e)
        {
            using (RestarauntContext db = new RestarauntContext())
            {
                if (NotAprovedDishesListBox.SelectedItems.Count == 1)
                {
                    DishListInfo dishToMove = (DishListInfo)NotAprovedDishesListBox.Items[NotAprovedDishesListBox.SelectedIndex];

                    notAprovedDishesCollection.RemoveAt(NotAprovedDishesListBox.SelectedIndex);
                    var dishToChange = db.Dishes.FirstOrDefault(d => d.Id == dishToMove.ID);
                    dishToChange.Status = true;
                    db.SaveChanges();
                    aprovedDishesCollection.Add(dishToMove);

                }
            }
        }

        private void OnCreateButton(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(PageController.IngridientCreationPage, UriKind.Relative);
        }

        private void OnEditIngridientButton(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(PageController.IngridientCreationPage, UriKind.Relative);
        }
        
        private void OnDeleteDishButton(object sender, RoutedEventArgs e)
        {
            using (RestarauntContext db = new RestarauntContext())
            {
                if (NotAprovedDishesListBox.SelectedItems.Count == 1)
                {
                    if (CanDelete())
                    {
                        DishListInfo dishToDelete = (DishListInfo)NotAprovedDishesListBox.Items[NotAprovedDishesListBox.SelectedIndex];
                        notAprovedDishesCollection.Remove(dishToDelete);
                        db.SaveChanges();
                        var findedDishLists = db.DishLists.Where(d => d.DishId == dishToDelete.ID).ToArray();
                        if (findedDishLists != null)
                        {
                            foreach (var dish in findedDishLists)
                            {
                                db.DishLists.Remove(dish);
                            }
                            db.SaveChanges();
                        }

                        var findedDishes = db.Dishes.Where(d => d.Id == dishToDelete.ID).ToList();
                        if (findedDishes != null)
                        {
                            foreach (var dish in findedDishes)
                            {
                                db.Dishes.Remove(dish);
                            }
                            db.SaveChanges();
                        }
                    }
                }
                else if(AprovedDishesListBox.SelectedItems.Count == 1) 
                {
                    MessageBox.Show("Блюдо можно удалить только из неутвержденных");
                }
            }
        }
        private bool CanDelete() 
        {
          CheckDeletingWindow checkDeletingWindow = new CheckDeletingWindow();
          checkDeletingWindow.ShowDialog();
            if (checkDeletingWindow.DialogResult == true)
            {
               return true;
            }
            else 
            {
                return false;
            }
            
        }
        private void OnEditDishButton(object sender, RoutedEventArgs e)
        {
            if (AprovedDishesListBox.SelectedItem != null)
            {
                PageController.SendDishListInfo((DishListInfo)AprovedDishesListBox.SelectedItem);
                NavigationService.Navigate(PageController.EditDish, UriKind.Relative);
            }
            else if(NotAprovedDishesListBox.SelectedItem != null) 
            {
                PageController.SendDishListInfo((DishListInfo)NotAprovedDishesListBox.SelectedItem);
                NavigationService.Navigate(PageController.EditDish, UriKind.Relative);
            }

            
        }

        private void OnCreateDishButton(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(PageController.DishCreationPage, UriKind.Relative);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void AprovedDishesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AprovedDishesListBox.Tag?.ToString() == "busy") return;
            NotAprovedDishesListBox.Tag = "busy";
            NotAprovedDishesListBox.SelectedIndex = -1;
            NotAprovedDishesListBox.Tag = null;
            //if (NotAprovedDishesListBox.SelectedItem != null)
            //{
            //    NotAprovedDishesListBox.SelectedItem = null;
            //}
        }

        private void NotAprovedDishesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotAprovedDishesListBox.Tag?.ToString() == "busy") return;
            AprovedDishesListBox.Tag = "busy";
            AprovedDishesListBox.SelectedIndex = -1;
            AprovedDishesListBox.Tag = null;
            //if (AprovedDishesListBox.SelectedItem != null)
            //{
            //    AprovedDishesListBox.SelectedItem = null;
            //}
        }
    }
}
