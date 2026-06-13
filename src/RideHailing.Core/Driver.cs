namespace RideHailing.Core
{
    public class Driver
    {
        public int Id { get; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public Driver(int id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public void UpdateLocation(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}