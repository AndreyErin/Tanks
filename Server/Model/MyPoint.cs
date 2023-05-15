
namespace Server.Model
{
    public class MyPoint
    {
        protected MyPoint() { }
        public MyPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
        public double X = 0;
        public double Y = 0;
    }
}
