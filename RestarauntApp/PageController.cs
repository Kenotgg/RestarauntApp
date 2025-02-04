using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestarauntApp
{
    internal class PageController
    {

        private static IngridientCreationPage _ingridientCreationPage = null!;
        public static IngridientCreationPage IngridientCreationPage
        {
            get 
            {
                if(_ingridientCreationPage == null) 
                {
                    _ingridientCreationPage = new IngridientCreationPage();
                }
                return _ingridientCreationPage;
            }
            
        }


        private static IngridientsListPage _ingridientsListPage = null!;
        public static IngridientsListPage IngridientsListPage
        {
            get
            {
                if (_ingridientsListPage == null)
                {
                    _ingridientsListPage = new IngridientsListPage();
                }
                return _ingridientsListPage;
            }
        }
    }
}
