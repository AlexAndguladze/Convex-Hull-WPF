using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConvexHull
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rand = new Random();
        List<Point> points = new();
        Stack<Point> stack = new();
        public MainWindow()
        {
            InitializeComponent();
        }
        void GeneratePoints()
        {
            int previousY = 0;
            for (int x = 60; x < canvas.ActualWidth-60; x+=rand.Next(15, 50))
            {
                int y = RandomYCoordinate(previousY);
                var ellipse = new Ellipse { Height = 10, Width = 10, StrokeThickness = 2, Stroke = Brushes.Black };
                Canvas.SetBottom(ellipse, y);
                Canvas.SetLeft(ellipse, x);
                points.Add(new Point(ellipse, x+5, y+5));
                canvas.Children.Add(ellipse);
                previousY = y;
            }
        }
        int FindLowestPoint()
        {
            int minY = points.Min(point => point.Y);
            IEnumerable<int>indexes=points.Select((point, index) => new {point, index })
                                          .Where(p=>p.point.Y==minY).Select(point=>point.index);
            return indexes.Last();//returns index of lowest point, if there are more than 1 return last one
            //https://stackoverflow.com/questions/31110652/linq-index-of-min-value-of-one-property-of-a-custom-object-c-sharp
        }
        void SortPoints()
        {
            points=points.OrderBy(point=>CalculateAngle(point.X, point.Y)).ToList();
        }
        private double CalculateAngle(int x, int y)
        {
            var lowestPoint = points[FindLowestPoint()];
            var xDiff = x - lowestPoint.X;
            var yDiff = y - lowestPoint.Y;
            var magnitude = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
            return Math.Acos(xDiff/ magnitude)*180/Math.PI;
        }
        int RandomYCoordinate(int previousY)
        {
            int y = rand.Next(30, (int)canvas.ActualHeight-30);
            if(Math.Abs(y- previousY) < 90)
            {
                y = RandomYCoordinate(previousY);
            }
            return y;
        }

        private void GeneratePointsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (canvas.Children.Count > 0)
            {
                canvas.Children.Clear();
                points.Clear();
                if(stack.Count > 0)
                stack.Clear();
            }
            GeneratePoints();
        }

        private void ConvexHullBtn_Click(object sender, RoutedEventArgs e)
        { 
            int index=FindLowestPoint();
            var lowestP=points[index];
            SortPoints();
           
            stack.Push(points[0]);
            stack.Push(points[1]);

            for (int i = 2; i < points.Count; i++)
            {
                Point next = points[i];
                Point p=stack.Pop();
                while(stack.Peek() !=null && ccw(stack.Peek(), p, next) <= 0)
                {
                    p=stack.Pop();
                }
                stack.Push(p);
                stack.Push(next);
            }
            var hull = stack.ToArray();
            for(int i = 0; i < stack.Count; i++)
            {
                hull[i].Ellipse.StrokeThickness = 4;
            }
            Polygon polygon = new Polygon();
            polygon.StrokeThickness = 4;
            for(int i=0; i < stack.Count; i++)
            {
                polygon.Points.Add(new System.Windows.Point(hull[i].X, canvas.ActualHeight-hull[i].Y));
            }
            polygon.Stroke=Brushes.Black;
            canvas.Children.Add(polygon);
        }
        int ccw(Point a, Point b, Point c)
        {
            float area = (b.X - a.X)*(c.Y-a.Y)-(b.Y-a.Y)*(c.X-a.X);
            if (area < 0) return -1;//clockwise
            else if (area > 0) return 1;
            else return 0;
        }
        private void Text(double x, double y, double text)
        {
            //text *= 180 / Math.PI;
            TextBlock textBlock = new TextBlock();
            //textBlock.Text = $"x:{x} y:{y} {text.ToString().Substring(0,3)}";
            textBlock.Text = text.ToString();
            textBlock.FontSize= 14;
            Canvas.SetLeft(textBlock, x);
            Canvas.SetBottom(textBlock, y);
            canvas.Children.Add(textBlock);
        }

    }
}
