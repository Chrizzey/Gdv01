namespace GdvAlgorithms
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ClippingWindow.Set(600, 100, 400, 200);

            var p0 = new Point2d(80, 60);
            var p1 = new Point2d(800, 600);
            
            Console.WriteLine("P0: " + p0);
            Console.WriteLine("P1: " + p1);

            var accept = LineClipping.CohenSutherland(p0, p1);

            Console.WriteLine("Accept: " + accept);
            Console.WriteLine("P0: " + p0);
            Console.WriteLine("P1: " + p1);
        }
    }
}