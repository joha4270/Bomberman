using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Bomberman
{

    /// <summary>
    /// This class serves 2 purposes
    /// First one of those is a char array that tracks changes (we don't have
    /// performance to redraw the entire screen each frame) 
    /// Second is to handle rendering of the screen
    /// </summary>
    class Renderable
    {
        protected Renderable Parent = null;
        public bool Enable { get; set; }
        protected internal readonly List<Renderable> Children = new List<Renderable>();
        protected internal CharInfo[,] Stuff;
        public Point Location {get; protected set;}
        protected internal readonly List<Point> Updates = new List<Point>();

        public Point Size
        {
            get
            {
                return new Point(Stuff.GetLength(0), Stuff.GetLength(1));
            }
        }
        public Renderable(int xSize, int ySize)
        {
            Resize(xSize,ySize);
            Location = Point.Zero;
            Enable = true;
        }
        public Renderable(Point size) : this(size.x, size.y) { }
        
        public Renderable AddChild(Point location, Point size)
        {
            Renderable child = new Renderable(size.x, size.y) {Location = location, Parent = this};
            Children.Add(child);
            return child;
        }

        public void AddChild(Point location, Renderable child)
        {
            child.Location = location;
            child.Parent = this;
            Children.Add(child);

        }

        public void Resize(int xSize,int ySize)
        {
            CharInfo[,] newstuff = new CharInfo[xSize, ySize];
            if(Stuff != null)
            {
                for (int x = 0; x < Math.Min(xSize,Stuff.GetLength(0)); x++)
			    {
                    for (int y = 0; y < Math.Min(ySize,Stuff.GetLength(1)); y++)
			        {
                        newstuff[x,y] = Stuff[x,y];
                    }
			    }
            }
            else
            {
                for (int x = 0; x < xSize; x++)
                {
                    for (int y = 0; y < ySize; y++)
                    {
                        newstuff[x,y] = new CharInfo('\0');
                    }
                }
            }
            Stuff = newstuff;
        }

        public virtual CharInfo this[int x, int y]
        {
            get
            {
                return Stuff[x, y];
            }
            set
            {
                Stuff[x, y] = value;
                //A lot of objects are going to be created here, static array 
                //and reusing objects is better ofr performance
                Updates.Add(new Point(x, y));
            }
        }

        public bool RenderHere(Point p)
        {
            if (Enable == false) return false;
            Point localPoint = p - Location;
            if (Stuff[localPoint.x, localPoint.y].Char.AsciiChar !=  0)
                return true;
            else
            {
                return false;
            }
        }

        public bool RenderHereRecursive(Point p)
        {
            if (RenderHere(p)) return true;

            Point localPoint = p - Location;

            return Children.Any(r => r.RenderHereRecursive(localPoint));
        }
        
        protected Renderable IsChild(Point p)
        {
            foreach (Renderable child in Children)
            {
                if(!(child.Location.x < p.x))
                    continue;
                if(!(child.Location.y < p.y))
                    continue;
                if(!(child.Location.x + child.Size.x > p.x))
                    continue;
                if (!(child.Location.y + child.Size.y > p.y))
                    continue;
                if(child.RenderHereRecursive(p))
                    continue;
                ;
                return child;


            }
            return null;
            return Children.FirstOrDefault(
                r => r.Enable && 
                r.Location.x < p.x && 
                r.Location.y < p.y && 
                r.Location.x + r.Size.x > p.x && 
                r.Location.y + r.Size.y > p.y && 
                !r.RenderHereRecursive(p));
        }

        /// <summary>
        /// Renderes this renderable to the screen
        /// </summary>
        /// <returns>Number of cells updated</returns>
        public virtual int Update(bool forceAll = false)
        {
            if (!Enable) return 0;
            //defined here and not inside the loop to prevent it from being
            //redefined each time loop runs
            //No need to use the stack more
            //Dunno if it gets optimized away anyway
            Renderable child = null;
            int count = 0;
            
            //if(forceAll)
            //{
                
            //    for (int x = 0; x < _stuff.GetLength(0); x++)
            //    {
            //        for (int y = 0; y < _stuff.GetLength(1); y++)
            //        {
            //            Point p = new Point(x,y);
            //            child = IsChild(p);
            //            if (child == null || !child.Enable || !child.RenderHereRecursive(p))
            //            {
            //                UpdatePoint(new Point(x, y));
            //                count++;
            //            }
            //            else
            //            {
            //                ;
            //            }
            //        }
            //    }
            //}


            if (forceAll)
            {
                for (int x = 0; x < Stuff.GetLength(0); x++)
                {
                    for (int y = 0; y < Stuff.GetLength(1); y++)
                    {
                        Updates.Add(new Point(x, y));
                    }
                }
            }

            foreach (Point p in Updates)
            {
                child = IsChild(p);
                if (child == null || !child.Enable )
                {
                    count++;
                    UpdatePoint(p);
                }
                else
                {
                    ;
                }

                
            }


            Updates.Clear();
            foreach(Renderable r in Children)
            {
                r.Update(forceAll);
            }

            return count;
        }

        protected void UpdatePoint(Point p)
        {
            Console.ForegroundColor = Stuff[p.x, p.y].ForegroundColor;
            Console.BackgroundColor = Stuff[p.x, p.y].BackgroundColor;
            Console.SetCursorPosition(RealPos(p).x, RealPos(p).y);
            Console.Write(Stuff[p.x, p.y].Char.AsciiChar);
        }

        protected Point RealPos(Point p)
        {
            Renderable w = this;
            do
            {
                p += w.Location;
            }
            while ((w = w.Parent) != null);
            return p;
        }

    }
}
