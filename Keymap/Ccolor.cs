using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keymap
{
    class Ccolor
    {
        public static void std()
        {
            for (int i = 0; i < 16; i++)
            {
                Console.WriteLine(i + " " +((ConsoleColor)i).ToString());
            }
        }
    }
}
