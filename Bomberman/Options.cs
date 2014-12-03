using System;

namespace Bomberman
{
    internal static class Options
    {
        internal static long BombFuse = 3000;
        internal static CharInfo BombDisplay = new CharInfo('O', ConsoleColor.Red, ConsoleColor.Black);
        internal static CharInfo BombGfx = new CharInfo('X', ConsoleColor.Yellow, ConsoleColor.Black);
        internal static long BombGfxTime = 300;
    }
}
