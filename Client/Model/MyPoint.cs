
namespace Client.Model
{
    public class MyPoint
    {
        public MyPoint() { }
        public MyPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; } = 0;

        public double Y { get; set; } = 0;

        public override string ToString()
        {
            return $"[{X},{Y}]";
        }
    }
}
