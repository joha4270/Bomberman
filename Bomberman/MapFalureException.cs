using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman
{
    class MapFalureException : Exception
    {
        public int line;
        private int line1;
        private Exception ex;

        

        public MapFalureException(int line1, Exception ex) : base(String.Format("Failed to read map file near line {0}", line1), ex)
        {
            
        }
    }
}
