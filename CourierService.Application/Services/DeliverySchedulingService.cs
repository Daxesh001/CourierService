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

            try
            {
                var result = CalculatePackageCosts(baseCost, packages);

                if (vehicle == null || vehicle.NumberOfVehicles <= 0)
                {
                    result.MaxDeliveryTimeHours = 0m;
                    return result;
                }

                AssignEtas(result, packages, vehicle);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ValidateInputs(decimal baseCost, List<Package> packages, Vehicle vehicle)
        {
            Guard.NonNegative(baseCost, nameof(baseCost));
            Guard.NotNullOrEmpty(packages, nameof(packages));

            foreach (var p in packages)
            {
                Guard.NotNull(p, nameof(packages));
                Guard.NonNegative(p.WeightKg, nameof(p.WeightKg));
                Guard.NonNegative(p.DistanceKm, nameof(p.DistanceKm));
            }

            if (vehicle != null)
            {
                Guard.GreaterThanZero(vehicle.MaxSpeedKmH, nameof(vehicle.MaxSpeedKmH));
                Guard.GreaterThanZero(vehicle.MaxCarryWeightKg, nameof(vehicle.MaxCarryWeightKg));
                Guard.GreaterThanZero(vehicle.NumberOfVehicles, nameof(vehicle.NumberOfVehicles));
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

        private void AssignEtas(DeliveryResultDto result, List<Package> packages, Vehicle vehicle)
        {
            var speed = vehicle.MaxSpeedKmH;
            var capacity = vehicle.MaxCarryWeightKg;
            var vehicleCount = vehicle.NumberOfVehicles;

            var vehicleAvailable = new decimal[vehicleCount];

            // Create batches (first-fit descending)
            var remaining = new List<Package>(packages.OrderByDescending(p => p.WeightKg));
            var batches = new List<List<Package>>();

            while (remaining.Any())
            {
                var batch = new List<Package>();
                var currentWeight = 0m;

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
        }
    }
}
