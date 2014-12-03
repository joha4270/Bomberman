using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman
{
    class MenuItem
    {
        public delegate void MenuAction
        (
            Menu m, 
            MenuItem navigatingTo, 
            MenuItem navigatingFrom,
            NavigationOption n
        );

        public object StateDataObject;
        public Point Position;
        public String Text;
        public MenuAction onExecute, onSelected;
        public ConsoleColor SelectedFg, SelectedBg, UnselectedFg, UnselectedBg;
        public MenuItem Above ,Below , Left , Right;
        public MenuItem
        (
            Point position, 
            String Text, 
            ConsoleColor SelectedFg, 
            ConsoleColor SelectedBg, 
            ConsoleColor UnselectedFg, 
            ConsoleColor UnselectedBg,
            MenuAction onExecute = null,
            MenuAction onSelected = null,
            MenuItem Above = null,
            MenuItem Below = null,
            MenuItem Left = null,
            MenuItem Right = null,
            object StateDataObject = null
        )
        {
            this.Position = position;
            this.Text = Text;
            this.SelectedFg = SelectedFg;
            this.SelectedBg = SelectedBg;
            this.UnselectedFg = UnselectedFg;
            this.UnselectedBg = UnselectedBg;
            this.onExecute = onExecute;
            this.onSelected = onSelected;
            this.Above = Above;
            this.Below = Below;
            this.Left = Left;
            this.Right = Right;
            this.StateDataObject = StateDataObject;
        }
    }
}
