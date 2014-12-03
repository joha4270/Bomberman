using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman
{
    class Renderes
    {
        public static Renderable Background;
        public static Renderable GameField;
        public static EntityRender EntityLayer;
        public static Renderable Menu;
        public static Renderable FpsDisplay;

        public static void Setup()
        {
            Background = new DirectRender(79, 37);
            GameField = Background.AddChild(new Point(2, 6), new Point(75, 29));
            GameField.Enable = false;
            EntityLayer = new EntityRender(75, 29);
            GameField.AddChild(new Point(0, 0), EntityLayer);
            FpsDisplay = Background.AddChild(new Point(0, 0), new Point(20, 2));
            FpsDisplay.Enable = false;

            Menu = Background.AddChild(new Point(5, 12), new Point(20, 10));
            Menu.Enable = false;


        }
    }
}
