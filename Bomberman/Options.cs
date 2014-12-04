using System;

namespace Bomberman
{
    internal static class Options
    {
        internal static long BombFuse = 3000;
        internal static CharInfo BombDisplay = new CharInfo('O', ConsoleColor.Red, ConsoleColor.Black);
        internal static CharInfo BombGfx = new CharInfo('X', ConsoleColor.Yellow, ConsoleColor.Black);
        internal static long BombGfxTime = 300;
        public static CharInfo BombAmountUpDisplay = new CharInfo('O', ConsoleColor.Red, ConsoleColor.Blue);
        public static CharInfo BombPowerUpDisplay = new CharInfo('+', ConsoleColor.Red, ConsoleColor.Blue);
        public static CharInfo HpupDisplay = new CharInfo(3, 0x9C);
        public static CharInfo BootsDisplay = new CharInfo(28, 0x9C);
    }
}
