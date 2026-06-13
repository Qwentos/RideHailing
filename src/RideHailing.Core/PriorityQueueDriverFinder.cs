using System.Collections.Generic;
using System.Linq;

namespace RideHailing.Core
{
    public class PriorityQueueDriverFinder : IDriverFinder
    {
        private readonly Map _map;

        public PriorityQueueDriverFinder(Map map)
        {
            _map = map;
        }

        public IReadOnlyList<Driver> FindNearestDrivers(Order order, int count = 5)
        {
            // Используем приоритет: (расстояние, ID). Компаратор обеспечивает,
            // что при равных расстояниях больший ID считается "хуже" и удаляется первым.
            var queue = new PriorityQueue<Driver, (double distance, int id)>(new DistanceIdComparer());

            foreach (var driver in _map.GetAllDrivers())
            {
                double dist = System.Math.Sqrt(
                    System.Math.Pow(driver.X - order.X, 2) +
                    System.Math.Pow(driver.Y - order.Y, 2));

                if (queue.Count < count)
                {
                    queue.Enqueue(driver, (dist, driver.Id));
                }
                else if (queue.TryPeek(out _, out var maxPrio) &&
                         (dist < maxPrio.distance ||
                          (dist == maxPrio.distance && driver.Id < maxPrio.id)))
                {
                    queue.Dequeue();
                    queue.Enqueue(driver, (dist, driver.Id));
                }
            }

            var result = new List<Driver>(count);
            while (queue.TryDequeue(out var driver, out _))
                result.Add(driver);
            result.Reverse(); // от ближайшего к дальнему
            return result.AsReadOnly();
        }

        // Компаратор: сначала по расстоянию (убывание), затем по ID (убывание)
        private class DistanceIdComparer : IComparer<(double distance, int id)>
        {
            public int Compare((double distance, int id) x, (double distance, int id) y)
            {
                int distComp = y.distance.CompareTo(x.distance);
                if (distComp != 0) return distComp;
                return y.id.CompareTo(x.id);
            }
        }
    }
}