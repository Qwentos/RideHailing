using System;
using System.Collections.Generic;
using System.Linq;

namespace RideHailing.Core
{
    public class GridSpiralDriverFinder : IDriverFinder
    {
        private readonly Map _map;
        private readonly int _width;
        private readonly int _height;
        private readonly Dictionary<(int, int), Driver> _grid;

        public GridSpiralDriverFinder(Map map, int width, int height)
        {
            _map = map;
            _width = width;
            _height = height;
            _grid = new Dictionary<(int, int), Driver>();
            foreach (var d in _map.GetAllDrivers())
                _grid[(d.X, d.Y)] = d;
        }

        public IReadOnlyList<Driver> FindNearestDrivers(Order order, int count = 5)
        {
            if (_grid.Count == 0)
                return new List<Driver>().AsReadOnly();

            var found = new List<(Driver driver, double distance)>();
            double bestMaxDist = double.MaxValue;

            int radius = 0;
            int maxRadius = Math.Max(_width, _height);

            while (radius <= maxRadius)
            {
                int minX = order.X - radius;
                int maxX = order.X + radius;
                int minY = order.Y - radius;
                int maxY = order.Y + radius;

                // верхняя и нижняя горизонтальные границы
                for (int x = minX; x <= maxX; x++)
                {
                    CheckCell(x, minY, order, found);
                    if (minY != maxY)
                        CheckCell(x, maxY, order, found);
                }
                // левая и правая вертикальные границы (без углов)
                for (int y = minY + 1; y <= maxY - 1; y++)
                {
                    CheckCell(minX, y, order, found);
                    if (minX != maxX)
                        CheckCell(maxX, y, order, found);
                }

                if (found.Count >= count)
                {
                    bestMaxDist = found.Max(f => f.distance);
                    // минимальное возможное расстояние до клеток за пределами текущего радиуса
                    if (bestMaxDist <= radius + 1)
                        break;
                }

                radius++;
            }

            return found
                .OrderBy(f => f.distance)
                .Take(count)
                .Select(f => f.driver)
                .ToList()
                .AsReadOnly();
        }

        private void CheckCell(int x, int y, Order order, List<(Driver driver, double distance)> found)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                return;

            if (_grid.TryGetValue((x, y), out var driver))
            {
                double dist = Math.Sqrt(Math.Pow(x - order.X, 2) + Math.Pow(y - order.Y, 2));
                found.Add((driver, dist));
            }
        }
    }
}