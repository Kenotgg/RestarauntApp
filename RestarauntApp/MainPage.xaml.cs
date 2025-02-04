using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        List<ListBox> listBoxes = new List<ListBox>();
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
                //var dishList = from dish in db.Dishes
                //               join singleDishList in db.DishLists on dish.Id equals singleDishList.DishId
                //               join ingridient in db.Ingridients on singleDishList.IngridientId equals ingridient.Id
                //               select new
                //               {
                //                   ID = singleDishList.Id,
                //                   DishTitle = dish.Title,
                //                   IngridietnTitle = ingridient.Title,
                //                   CountOfIngridients = singleDishList.Count
                //               };

                var dishes = db.Dishes.Select(p => new
                {
                    Title = p.Title,
                    Weight = p.Weight,
                    Kcal = p.KiloCalories,
                    Status = p.Status,
                    Proteins = p.Proteins,
                    Fats = p.Fats,
                    Cabohydrates = p.Carbohydrates
                }
                );

                listBoxes.Add(ListBox1);
                listBoxes.Add(ListBox2);
                listBoxes.Add(ListBox3);
                listBoxes.Add(ListBox4);
                listBoxes.Add(ListBox5);
                listBoxes.Add(ListBox6);
                int indexer = 0;
                foreach (var d in dishes)
                {

                    if (listBoxes[indexer].Items.Count < 4)
                    {
                        listBoxes[indexer].Items.Add($"Название: {d.Title}");
                        listBoxes[indexer].Items.Add($"Вес: {d.Weight} грамм");
                        listBoxes[indexer].Items.Add($"ККАЛ: {d.Kcal}");
                        listBoxes[indexer].Items.Add($"БЖУ: {d.Proteins} {d.Fats} {d.Cabohydrates}");
                    }

                    if (indexer <= listBoxes.Count)
                    {
                        indexer++;
                    }

                }
                for (int i = 0; i < listBoxes.Count; i++)
                {
                    if (listBoxes[i].Items.Count < 4)
                    {
                        if (listBoxes[i] != null)
                        {
                            AprovedPanel.Children.Remove(listBoxes[i]);
                            if (listBoxes[i] != null)
                            {
                                NotAprovedPanel.Children.Remove(listBoxes[i]);
                            }
                        }

                    }

                }
            }



        }

        private void onRightButton(object sender, RoutedEventArgs e)
        {
            int indexer = 0;

            foreach (var l in listBoxes)
            {


                if (listBoxes[indexer].SelectedItem != null && listBoxes[indexer].Parent != NotAprovedPanel)
                {

                    AprovedPanel.Children.Remove(listBoxes[indexer]);
                    NotAprovedPanel.Children.Add(listBoxes[indexer]);
                    listBoxes[indexer].SelectedItem = null;

                }
                if (indexer <= listBoxes.Count)
                {
                    indexer++;
                }


            }


        }

        private void onLeftButton(object sender, RoutedEventArgs e)
        {
            int indexer = listBoxes.Count - 1;

            foreach (var l in listBoxes)
            {


                if (listBoxes[indexer].SelectedItem != null && listBoxes[indexer].Parent != AprovedPanel)
                {
                    NotAprovedPanel.Children.Remove(listBoxes[indexer]);
                    AprovedPanel.Children.Add(listBoxes[indexer]);
                    listBoxes[indexer].SelectedItem = null;

                }
                if (indexer >= 0)
                {
                    indexer--;
                }


            }
        }

        private void OnCreateButton(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(PageController.IngridientCreationPage, UriKind.Relative);
        }

        private void OnEditButton(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(PageController.IngridientCreationPage, UriKind.Relative);
        }

        private void OnDeleteButton(object sender, RoutedEventArgs e)
        {
            foreach (var l in listBoxes) 
            {
                if(l.SelectedItem != null && l.Parent == NotAprovedPanel) 
                {
                    NotAprovedPanel.Children.Remove(l);
                }
                else if (l.SelectedItem != null && l.Parent == AprovedPanel)
                {
                    AprovedPanel.Children.Remove(l);
                }

            }
        }

        private void OnOpenIngridientsList(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(PageController.IngridientsListPage, UriKind.Relative);
            
        }
    }
}
