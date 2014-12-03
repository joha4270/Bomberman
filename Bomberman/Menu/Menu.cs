using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman
{
    class Menu
    {
        private Renderable renderable;
        
        

        public Menu(Renderable renderable)
        {
            
            // TODO: Complete member initialization
            
            this.renderable = renderable;
        }

        internal bool HandleKey(ConsoleKeyInfo consoleKeyInfo)
        {
            return false;
        }

        internal void Setup()
        {
            
        }
    }
}
