using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;
namespace RestarauntApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GetData();
        }

        

        void GetData()
        {
            using (RestarauntContext db = new RestarauntContext())
            {

              
                var dishList = from dish in db.Dishes
                               join singleDishList in db.DishLists on dish.Id equals singleDishList.DishId
                               join ingridient in db.Ingridients on singleDishList.IngridientId equals ingridient.Id
                               select new
                               {
                                   ID = singleDishList.Id,
                                   DishTitle = dish.Title,
                                   IngridietnTitle = ingridient.Title,
                                   CountOfIngridients = singleDishList.Count
                               };

                foreach (var d in dishList)
                {
                    MainListBox.Items.Add(d);
                   
                }

            }
        }
    }
}