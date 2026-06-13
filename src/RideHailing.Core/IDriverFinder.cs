using System.Collections.Generic;

namespace RideHailing.Core
{
    public interface IDriverFinder
    {
        IReadOnlyList<Driver> FindNearestDrivers(Order order, int count = 5);
    }
}