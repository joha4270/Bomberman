using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Bomberman
{
    class DirectRender : Renderable
    {
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutput(
          SafeFileHandle hConsoleOutput,
          CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        protected SafeFileHandle Handle;
        protected CharInfo[] Buffer;

        public DirectRender(int xSize, int ySize) : base(xSize, ySize)
        {
            //Create file handle to the console buffer
            //Basicaly lets us writes bytes to the console buffer, accces is
            //trough a file handle
            Handle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            Buffer = new CharInfo[79 * 37];
        }

        public override int Update(bool forceAll = false)
        {
            if(!Handle.IsInvalid)
            {
                for (int x = 0; x < 79; x++)
                {
                    for (int y = 0; y < 37; y++)
                    {
                        Buffer[x + y * 79] = Stuff[x, y];
                    }
                }

                CopyChild(ref Buffer, Children, Location);

                SmallRect rect = new SmallRect() {Bottom = 37, Left = 0, Right = 79, Top = 0};
                bool Success = WriteConsoleOutput(Handle, Buffer,
                    new Coord(79, 37),
                    new Coord(0, 0),
                    ref rect);

                if (!Success)
                {
                    Trace.WriteLine("Error drawing console");
                }

            }
            else
            {
                Trace.WriteLine("Console handle invalid");
                throw new RenderException();
            }

            return 79*37;
        }

        private static void CopyChild(ref CharInfo[] buffer, IEnumerable<Renderable> children, Point offset)
        {
            foreach (Renderable child in children)
            {
                if(!child.Enable) continue;

                Point location = offset + child.Location;

                for (int x = 0; x < child.Size.x; x++)
                {
                    for (int y = 0; y < child.Size.y; y++)
                    {
                        if(child[x,y].Char.AsciiChar != 0)
                        {
                            buffer[(x + location.x) + (y + location.y)*79] = child[x, y];
                        }
                        else
                        {
                            ;
                        }
                    }
                }

                CopyChild(ref buffer, child.Children, location);
                child.Updates.Clear();
            }
        }
    }
}