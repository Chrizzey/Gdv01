namespace GdvAlgorithms
{
    public class Point2d
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point2d() { }

        public Point2d(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"({X}|{Y})";
    }

    public class Pixel2d
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Pixel2d() { }

        public Pixel2d(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
