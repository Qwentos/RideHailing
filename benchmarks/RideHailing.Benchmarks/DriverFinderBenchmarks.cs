using BenchmarkDotNet.Attributes;
using RideHailing.Core;
using System;
using System.Collections.Generic;

namespace RideHailing.Benchmarks
{
    [MemoryDiagnoser]
    public class DriverFinderBenchmarks
    {
        private Map _map;
        private Order _order;
        private int _size; // сохраняем реальный размер карты

        [Params(100, 1000, 10000)]
        public int DriverCount;

        [GlobalSetup]
        public void Setup()
        {
            var rand = new Random(42);
            _size = (int)Math.Ceiling(Math.Sqrt(DriverCount * 2));
            _size = Math.Max(_size, 100);
            _map = new Map(_size, _size);

            var used = new HashSet<(int, int)>();
            for (int i = 1; i <= DriverCount; i++)
            {
                int x, y;
                do
                {
                    x = rand.Next(_size);
                    y = rand.Next(_size);
                } while (!used.Add((x, y)));
                _map.AddDriver(new Driver(i, x, y));
            }

            _order = new Order(_size / 2, _size / 2);
        }

        [Benchmark]
        public IReadOnlyList<Driver> BruteForce()
        {
            return new BruteForceDriverFinder(_map).FindNearestDrivers(_order, 5);
        }

        [Benchmark]
        public IReadOnlyList<Driver> PriorityQueue()
        {
            return new PriorityQueueDriverFinder(_map).FindNearestDrivers(_order, 5);
        }

        [Benchmark]
        public IReadOnlyList<Driver> GridSpiral()
        {
            // Передаём реальный размер карты
            return new GridSpiralDriverFinder(_map, _size, _size).FindNearestDrivers(_order, 5);
        }
    }
}