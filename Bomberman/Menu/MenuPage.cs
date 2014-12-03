using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman
{
    class MenuPage
    {
        Menu menu;

        public void Draw()
        {

        }
        public void Clear()
        {

        }

        public void Navigate(NavigationOption n)
        {
            if (n != NavigationOption.Select)
            {
                if (n == NavigationOption.Down)
                {
                    NavigateTo(Selected.Below, n);
                }
                else if(n == NavigationOption.Up)
                {
                    NavigateTo(Selected.Above, n);
                }
                else if (n == NavigationOption.Left)
                {
                    NavigateTo(Selected.Left, n);

                }
                else if (n == NavigationOption.Right)
                {
                    NavigateTo(Selected.Right, n);
                }
            }
            else
            {
                if(Selected.onExecute != null)
                {
                    Selected.onExecute(menu, Selected, Selected,n);
                }
            }
        }

        public bool NavigateTo(MenuItem m, NavigationOption n)
        {
            if (m != null && m.onSelected != null)
            {
                m.onSelected(menu, m, Selected, n);
                return true;
            }
            return false;
        }

        private int selectedID;
        public MenuItem Selected
        {
            get
            {
                return items[selectedID];
            }
            set
            {
                if (items.Contains(value))
                {
                    selectedID = items.IndexOf(value);
                }
            }
        }
        List<MenuItem> items;
        public MenuPage(params MenuItem[] items)
        {
            this.items = new List<MenuItem>(items);
        }
    }
}
