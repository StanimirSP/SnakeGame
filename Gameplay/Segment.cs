using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Gameplay
{
    public class Segment
    {
        private IntPoint p, q;
        public Segment() : this(new IntPoint(), new IntPoint()) { }
        public Segment(IntPoint p, IntPoint q)
        {
            P = p;
            Q = q;
        }
        public IntPoint P
        {
            get => new IntPoint(p.X, p.Y);
            set => p = value != null ? new IntPoint(value.X, value.Y) : throw new InvalidOperationException();
        }
        public IntPoint Q
        {
            get => new IntPoint(q.X, q.Y);
            set => q = value != null ? new IntPoint(value.X, value.Y) : throw new InvalidOperationException();
        }
        public static bool Intersect(Segment a, Segment b)
        {
            int o1 = IntPoint.Orientation(a.p, a.q, b.p);
            int o2 = IntPoint.Orientation(a.p, a.q, b.q);
            int o3 = IntPoint.Orientation(b.p, b.q, a.p);
            int o4 = IntPoint.Orientation(b.p, b.q, a.q);
            // General case 
            if (o1 != o2 && o3 != o4)
                return true;
            // Special Cases 
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
            if (o1 == 0 && b.p.OnSegment(a)) return true;
            // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
            if (o2 == 0 && b.q.OnSegment(a)) return true;
            // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
            if (o3 == 0 && a.p.OnSegment(b)) return true;
            // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
            if (o4 == 0 && a.q.OnSegment(b)) return true;
            return false;
        }
    }
}
