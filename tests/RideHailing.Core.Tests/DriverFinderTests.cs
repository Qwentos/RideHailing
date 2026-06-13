using NUnit.Framework;
using RideHailing.Core;
using System.Collections.Generic;
using System.Linq;

namespace RideHailing.Core.Tests
{
    [TestFixture]
    public class DriverFinderTests
    {
        private Map _map;

        [SetUp]
        public void Setup()
        {
            _map = new Map(10, 10);
            _map.AddDriver(new Driver(1, 1, 1));
            _map.AddDriver(new Driver(2, 3, 3));
            _map.AddDriver(new Driver(3, 5, 5));
            _map.AddDriver(new Driver(4, 8, 8));
            _map.AddDriver(new Driver(5, 2, 2));
            _map.AddDriver(new Driver(6, 9, 9));
            _map.AddDriver(new Driver(7, 0, 9));
            _map.AddDriver(new Driver(8, 5, 1));
        }

        [Test]
        public void BruteForce_FindsFiveNearest()
        {
            var finder = new BruteForceDriverFinder(_map);
            var nearest = finder.FindNearestDrivers(new Order(4, 4));
            Assert.That(nearest.Count, Is.EqualTo(5));
            Assert.That(nearest.Select(d => d.Id), Is.EquivalentTo(new[] { 5, 2, 3, 1, 8 }));
        }

        [Test]
        public void PriorityQueue_FindsFiveNearest()
        {
            var finder = new PriorityQueueDriverFinder(_map);
            var nearest = finder.FindNearestDrivers(new Order(4, 4));
            Assert.That(nearest.Count, Is.EqualTo(5));
            Assert.That(nearest.Select(d => d.Id), Is.EquivalentTo(new[] { 5, 2, 3, 1, 8 }));
        }

        [Test]
        public void GridSpiral_FindsFiveNearest()
        {
            var finder = new GridSpiralDriverFinder(_map, 10, 10);
            var nearest = finder.FindNearestDrivers(new Order(4, 4));
            Assert.That(nearest.Count, Is.EqualTo(5));
            Assert.That(nearest.Select(d => d.Id), Is.EquivalentTo(new[] { 5, 2, 3, 1, 8 }));
        }

        [Test]
        public void AllAlgorithms_ReturnSameResult_OnVariousOrders()
        {
            var finders = new IDriverFinder[]
            {
                new BruteForceDriverFinder(_map),
                new PriorityQueueDriverFinder(_map),
                new GridSpiralDriverFinder(_map, 10, 10)
            };

            var orders = new[] { new Order(0, 0), new Order(9, 9), new Order(4, 5) };
            foreach (var order in orders)
            {
                var referenceSet = new HashSet<int>(finders[0].FindNearestDrivers(order).Select(d => d.Id));
                foreach (var finder in finders.Skip(1))
                {
                    var idsSet = new HashSet<int>(finder.FindNearestDrivers(order).Select(d => d.Id));
                    Assert.That(idsSet, Is.EquivalentTo(referenceSet),
                        $"Mismatch at order ({order.X},{order.Y}) for {finder.GetType().Name}");
                }
            }
        }
    }
}