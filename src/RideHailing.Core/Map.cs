using System;
using System.Collections.Generic;
using System.Linq;

namespace RideHailing.Core
{
    public class Map : IDriverFinder
    {
        private readonly int _width;
        private readonly int _height;
        private readonly Dictionary<int, Driver> _drivers = new();

        public Map(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public void AddDriver(Driver driver)
        {
            if (driver.X < 0 || driver.X >= _width || driver.Y < 0 || driver.Y >= _height)
                throw new ArgumentOutOfRangeException(nameof(driver), "Coordinates out of map bounds");
            _drivers[driver.Id] = driver;
        }

        public void RemoveDriver(int id) => _drivers.Remove(id);

        public Driver? GetDriver(int id) => _drivers.TryGetValue(id, out var d) ? d : null;

        public IReadOnlyList<Driver> GetAllDrivers() => _drivers.Values.ToList().AsReadOnly();

        public IReadOnlyList<Driver> FindNearestDrivers(Order order, int count = 5)
        {
            var finder = new PriorityQueueDriverFinder(this);
            return finder.FindNearestDrivers(order, count);
        }
    }
}