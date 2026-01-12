using CourierService.Application.DTOs;
using CourierService.Application.Interfaces;
using CourierService.Domain.Entities;

namespace CourierService.Application.Services
{
    public class DeliverySchedulingService : IDeliveryScheduler
    {
        private readonly ICostCalculator _costCalculator;
        private readonly IOfferService _offerService;

        public DeliverySchedulingService(ICostCalculator costCalculator, IOfferService offerService)
        {
            _costCalculator = costCalculator;
            _offerService = offerService;
        }

        public DeliveryResultDto ScheduleAndCalculate(decimal baseCost, List<Package> packages, Vehicle vehicle)
        {
            var result = new DeliveryResultDto();

            // Calculate delivery cost and discount per package
            foreach (var pkg in packages)
            {
                var deliveryCost = _costCalculator.CalculateDeliveryCost(baseCost, pkg.WeightKg, pkg.DistanceKm);
                var discountPercent = _offerService.GetDiscountPercent(pkg.OfferCode, pkg);
                var discount = (discountPercent / 100m) * deliveryCost;
                var total = deliveryCost - discount;

                result.Packages.Add(new PackageResultDto
                {
                    Id = pkg.Id,
                    DeliveryCost = decimal.Round(deliveryCost, 2),
                    Discount = decimal.Round(discount, 2),
                    TotalCost = decimal.Round(total, 2),
                    ETAHours = 0m
                });

                result.TotalCost += total;
            }

            if (vehicle == null || vehicle.NumberOfVehicles <= 0)
            {
                // if no vehicle info provided, return costs without ETA
                result.MaxDeliveryTimeHours = 0m;
                return result;
            }

            var speed = vehicle.MaxSpeedKmH;
            var capacity = vehicle.MaxCarryWeightKg;
            var vehicleCount = vehicle.NumberOfVehicles;

            var vehicleAvailable = new decimal[vehicleCount];

            // Create batches
            var remaining = new List<Package>(packages.OrderByDescending(p => p.WeightKg));
            var batches = new List<List<Package>>();

            while (remaining.Any())
            {
                var batch = new List<Package>();
                var currentWeight = 0m;

                // simple first-fit descending
                for (int i = 0; i < remaining.Count; )
                {
                    if (currentWeight + remaining[i].WeightKg <= capacity)
                    {
                        currentWeight += remaining[i].WeightKg;
                        batch.Add(remaining[i]);
                        remaining.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }

                if (batch.Any()) batches.Add(batch);
                else break;
            }

            for (int i = 0; i < batches.Count; i++)
            {
                int vehicleIndex = 0;
                decimal earliest = vehicleAvailable[0];
                for (int v = 1; v < vehicleCount; v++)
                {
                    if (vehicleAvailable[v] < earliest)
                    {
                        earliest = vehicleAvailable[v];
                        vehicleIndex = v;
                    }
                }

                var batch = batches[i];
                var maxDistance = batch.Max(b => b.DistanceKm);

                var travelTime = maxDistance / speed;

                foreach (var pkg in batch)
                {
                    var pkgResult = result.Packages.Find(p => p.Id == pkg.Id);
                    if (pkgResult != null)
                    {
                        pkgResult.ETAHours = decimal.Round(earliest + (pkg.DistanceKm / speed), 2);
                    }
                }

                var roundTrip = (2 * maxDistance) / speed;
                vehicleAvailable[vehicleIndex] = decimal.Round(earliest + roundTrip, 2);
            }

            result.MaxDeliveryTimeHours = vehicleAvailable.Max();

            return result;
        }
    }
}
