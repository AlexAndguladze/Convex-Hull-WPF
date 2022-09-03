using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ConvexHull
{
    internal class Point
    {
        public Ellipse? Ellipse { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        
        public Point(Ellipse ellipse, int x, int y)
        {
            this.Ellipse = ellipse;
            this.X = x;
            this.Y = y;
        }

    }
}
