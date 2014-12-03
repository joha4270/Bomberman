using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keymap
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("k - Keymap");
            Console.WriteLine("c - Colors");
            char c = Console.ReadKey().KeyChar;

            switch(c)
            {
                case 'c':
                    Ccolor.std();
                    break;
                case 'k':
                    cdisp();
                    break;
                default:
                    Console.WriteLine("Not regoniced");
                    break;
            }

            Console.ReadLine();

        }

        private static void cdisp()
        {
            for (int i = 0; i < 256; i++)
            {
                
                Console.Write(i); Console.Write(' ');
                //Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.Write((char)i);
                Console.BackgroundColor = ConsoleColor.Black;
                if (i % 10 == 9)
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.Write("\t");
                }
            }
        }
    }
}
