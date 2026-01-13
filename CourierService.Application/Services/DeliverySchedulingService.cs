using CourierService.Application.Common;
using CourierService.Application.DTOs;
using CourierService.Application.Interfaces;
using CourierService.Domain.Entities;

namespace CourierService.Application.Services
{
    public class DeliverySchedulingService(ICostCalculator _costCalculator, IOfferService _offerService) : IDeliveryScheduler
    {
        public DeliveryResultDto ScheduleAndCalculate(decimal baseCost, List<Package> packages, Vehicle vehicle)
        {
            ValidateInputs(baseCost, packages, vehicle);

            var result = CalculatePackageCosts(baseCost, packages);

            if (vehicle == null || vehicle.NumberOfVehicles <= 0)
            {
                result.MaxDeliveryTimeHours = 0m;
                return result;
            }

            AssignEtas(result, packages, vehicle);

            return result;
        }

        private void ValidateInputs(decimal baseCost, List<Package> packages, Vehicle vehicle)
        {
            Guard.NonNegative(baseCost, nameof(baseCost));
            Guard.NotNullOrEmpty(packages, nameof(packages));

            foreach (var p in packages)
            {
                Guard.NotNull(p, nameof(p));
                Guard.NonNegative(p.WeightKg, nameof(p.WeightKg));
                Guard.NonNegative(p.DistanceKm, nameof(p.DistanceKm));
            }
        }

        private DeliveryResultDto CalculatePackageCosts(decimal baseCost, List<Package> packages)
        {
            var result = new DeliveryResultDto();

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

            return result;
        }
        private void AssignEtas(
            DeliveryResultDto result,
            List<Package> packages,
            Vehicle vehicle)
        {
            int vehicleCount = vehicle.NumberOfVehicles;
            decimal speed = vehicle.MaxSpeedKmH;
            decimal capacity = vehicle.MaxCarryWeightKg;

            var vehicleAvailableAt = new decimal[vehicleCount];
            var remaining = new List<Package>(packages);

            while (remaining.Any())
            {
                // Step 1: choose earliest available vehicle
                int vehicleIndex = GetNextAvailableVehicle(vehicleAvailableAt);
                decimal startTime = vehicleAvailableAt[vehicleIndex];

                // Step 2: pick best shipment
                var shipment = SelectBestShipment(remaining, capacity);

                // Step 3: assign ETA
                foreach (var pkg in shipment)
                {
                    var pkgResult = result.Packages
                        .First(p => p.Id == pkg.Id);

                    pkgResult.ETAHours = decimal.Round(startTime + (pkg.DistanceKm / speed), 2);
                }

                decimal maxDistance = shipment.Max(p => p.DistanceKm);
                decimal roundTripTime = (2 * maxDistance) / speed;

                vehicleAvailableAt[vehicleIndex] = decimal.Round(startTime + roundTripTime, 2);

                // Step 5: remove delivered packages
                remaining.RemoveAll(p => shipment.Contains(p));
            }

            result.MaxDeliveryTimeHours = vehicleAvailableAt.Max();
        }

        private List<Package> SelectBestShipment(List<Package> packages,decimal capacity)
        {
            List<Package> best = new();
            int bestCount = 0;
            decimal bestWeight = 0;
            decimal bestMaxDistance = decimal.MaxValue;

            int n = packages.Count;
            int combinations = 1 << n;

            for (int mask = 1; mask < combinations; mask++)
            {
                var current = new List<Package>();
                decimal weight = 0;

                for (int i = 0; i < n; i++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        weight += packages[i].WeightKg;
                        if (weight > capacity) break;
                        current.Add(packages[i]);
                    }
                }

                if (weight > capacity) continue;

                int count = current.Count;
                decimal maxDistance = current.Max(p => p.DistanceKm);

                if (count > bestCount ||
                    (count == bestCount && weight > bestWeight) ||
                    (count == bestCount && weight == bestWeight &&
                     maxDistance < bestMaxDistance))
                {
                    best = current;
                    bestCount = count;
                    bestWeight = weight;
                    bestMaxDistance = maxDistance;
                }
            }

            return best;
        }

        private int GetNextAvailableVehicle(decimal[] availableAt)
        {
            int index = 0;
            decimal earliest = availableAt[0];

            for (int i = 1; i < availableAt.Length; i++)
            {
                if (availableAt[i] < earliest)
                {
                    earliest = availableAt[i];
                    index = i;
                }
            }

            return index;
        }
    }
}
