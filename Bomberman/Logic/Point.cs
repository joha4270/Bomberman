using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman
{
    struct Point
    {
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Point && Equals((Point)obj);
        }

        public bool Equals(Point other)
        {
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x*0x18d) ^ y;
            }
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", x, y);

        }

        public static Point Zero
        {
            get
            {
                return new Point(0, 0);
            }
        }
        public Point(int X, int Y)
        {
            x = X;
            y = Y;
        }
        public int x;
        public int y;

        public static Point operator+(Point p, Point p2)
        {
            return new Point(p.x+p2.x, p.y + p2.y);
        }
        public static Point operator-(Point p, Point p2)
        {
            return new Point(p.x - p2.x, p.y - p2.y);
        }

        public static bool operator ==(Point p, Point p2)
        {
            return p.Equals(p2);
        }

        public static bool operator !=(Point p, Point p2)
        {
            return !p.Equals(p2);
        }

        public static Point operator *(Point p, int m)
        {
            return new Point(p.x * m, p.y * m);
        }
    }
}
