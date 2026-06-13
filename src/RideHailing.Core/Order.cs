namespace RideHailing.Core
{
    public class Order
    {
        public int X { get; }
        public int Y { get; }

        public Order(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}