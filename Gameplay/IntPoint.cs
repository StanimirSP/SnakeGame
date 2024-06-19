using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Gameplay
{
    public class IntPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public IntPoint(): this(0, 0) { }
        public IntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        // To find orientation of ordered triplet (p, q, r). 
        // The function returns following values 
        // 0 --> p, q and r are colinear 
        // 1 --> Clockwise 
        // 2 --> Counterclockwise 
        public static int Orientation(IntPoint p, IntPoint q, IntPoint r)
        {
            int val = (q.Y - p.Y) * (r.X - q.X) -
                    (q.X - p.X) * (r.Y - q.Y);

            if (val == 0) return 0; // colinear 

            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }
        // works if and only if all 3 points are collinear
        public bool OnSegment(Segment s)
        {
            if (X <= Math.Max(s.P.X, s.Q.X) && X >= Math.Min(s.P.X, s.Q.X) &&
                Y <= Math.Max(s.P.Y, s.Q.Y) && Y >= Math.Min(s.P.Y, s.Q.Y))
                return true;
            return false;
        }
        public static explicit operator IntPoint(Point p)
        {
            return new IntPoint((int)(10*p.X), (int)(10*p.Y)); // multiplying by 10 to increase precision
        }
        public static double Distance(Point p1, Point p2)
            => Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
    }
}
