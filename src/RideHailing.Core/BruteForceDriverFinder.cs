using System.Collections.Generic;
using System.Linq;

namespace RideHailing.Core
{
    public class BruteForceDriverFinder : IDriverFinder
    {
        private readonly Map _map;

        public BruteForceDriverFinder(Map map)
        {
            _map = map;
        }

        public IReadOnlyList<Driver> FindNearestDrivers(Order order, int count = 5)
        {
            return _map.GetAllDrivers()
                .OrderBy(d => System.Math.Sqrt(System.Math.Pow(d.X - order.X, 2) + System.Math.Pow(d.Y - order.Y, 2)))
                .Take(count)
                .ToList()
                .AsReadOnly();
        }
    }
}