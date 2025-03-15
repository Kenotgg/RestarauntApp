using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RestarauntApp
{
    internal class PageController
    {

        private static IngridientCreationPage _ingridientCreationPage = null!;
        public static IngridientCreationPage IngridientCreationPage
        {
            get
            {
                if (_ingridientCreationPage == null)
                {
                    _ingridientCreationPage = new IngridientCreationPage();
                }
                _ingridientCreationPage.ClearData();
                return _ingridientCreationPage;
            }

        }


        private static DishCreationPage _dishCreationPage = null!;
        public static DishCreationPage DishCreationPage
        {
            get
            {
                if (_dishCreationPage == null) 
                {
                    _dishCreationPage = new DishCreationPage();
                }
                _dishCreationPage.ClearData();
                _dishCreationPage.LoadData();
                return _dishCreationPage;
                
            }
        }
        public static DishListInfo _dishListInfo = null!;
        public static void SendDishListInfo(DishListInfo dishListInfo) 
        {
            _dishListInfo = dishListInfo;
        }
       
        private static EditDish _editDish = null!;
        public static EditDish EditDish 
        {
            get
            {
                if (_editDish == null)
                {
                    _editDish = new EditDish();
                   
                }
                _editDish.LoadIngridients();
                return _editDish;
            }
        }

    }
}
