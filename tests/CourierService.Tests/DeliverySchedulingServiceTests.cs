using CourierService.Application.Services;
using CourierService.Domain.Entities;
using System;
using System.Collections.Generic;
using Xunit;

namespace CourierService.Tests
{
    public class DeliverySchedulingServiceTests
    {
        [Fact]
        public void ScheduleAndCalculate_NoVehicle_ReturnsCostsWithoutETA()
        {
            var costSvc = new CostCalculationService();
            var offerSvc = new OfferService();
            var scheduler = new DeliverySchedulingService(costSvc, offerSvc);

            var packages = new List<Package>
            {
                new Package { Id = "PKG1", WeightKg = 50m, DistanceKm = 30m, OfferCode = null },
                new Package { Id = "PKG2", WeightKg = 100m, DistanceKm = 50m, OfferCode = "OFR003" }
            };

            var result = scheduler.ScheduleAndCalculate(100m, packages, null);

            // No vehicle info -> no ETA and MaxDeliveryTimeHours == 0
            Assert.Equal(0m, result.MaxDeliveryTimeHours);
            foreach (var p in result.Packages)
            {
                Assert.Equal(0m, p.ETAHours);
            }

            // Costs should be calculated
            Assert.Equal(2, result.Packages.Count);
            Assert.True(result.TotalCost > 0m);
        }

        [Fact]
        public void ScheduleAndCalculate_WithVehicles_ComputesETAsAndMaxDeliveryTime()
        {
            var costSvc = new CostCalculationService();
            var offerSvc = new OfferService();
            var scheduler = new DeliverySchedulingService(costSvc, offerSvc);

            var packages = new List<Package>
            {
                new Package { Id = "PKG1", WeightKg = 50m, DistanceKm = 30m, OfferCode = null },
                new Package { Id = "PKG2", WeightKg = 150m, DistanceKm = 150m, OfferCode = "OFR002" },
                new Package { Id = "PKG3", WeightKg = 100m, DistanceKm = 50m, OfferCode = "OFR003" }
            };

            var vehicle = new Vehicle { MaxSpeedKmH = 70m, MaxCarryWeightKg = 200m, NumberOfVehicles = 2 };

            var result = scheduler.ScheduleAndCalculate(100m, packages, vehicle);

            // Check totals (calculated manually)
            // PKG1: cost = 100 + 50*10 + 30*5 = 750
            // PKG2: cost = 100 + 150*10 + 150*5 = 2350 -> 7% discount = 164.5 -> total = 2185.5
            // PKG3: cost = 100 + 100*10 + 50*5 = 1350 -> 5% discount = 67.5 -> total = 1282.5
            Assert.Equal(4218.0m, result.TotalCost);

            // Verify ETAs and max delivery time
            var pkg1 = result.Packages.Find(p => p.Id == "PKG1");
            var pkg2 = result.Packages.Find(p => p.Id == "PKG2");
            var pkg3 = result.Packages.Find(p => p.Id == "PKG3");

            Assert.Equal(0.43m, pkg1.ETAHours);
            Assert.Equal(2.14m, pkg2.ETAHours);
            Assert.Equal(0.71m, pkg3.ETAHours);
            Assert.Equal(4.29m, result.MaxDeliveryTimeHours);
        }

        [Fact]
        public void ScheduleAndCalculate_NullPackages_ThrowsArgumentNullException()
        {
            var costSvc = new CostCalculationService();
            var offerSvc = new OfferService();
            var scheduler = new DeliverySchedulingService(costSvc, offerSvc);

            Assert.Throws<ArgumentNullException>(() => scheduler.ScheduleAndCalculate(100m, null, null));
        }

        [Fact]
        public void ScheduleAndCalculate_InvalidVehicle_ThrowsArgumentException()
        {
            var costSvc = new CostCalculationService();
            var offerSvc = new OfferService();
            var scheduler = new DeliverySchedulingService(costSvc, offerSvc);

            var packages = new List<Package>
            {
                new Package { Id = "PKG1", WeightKg = 50m, DistanceKm = 30m, OfferCode = null }
            };

            var invalidVehicle = new Vehicle { MaxSpeedKmH = 0m, MaxCarryWeightKg = 200m, NumberOfVehicles = 1 };

            Assert.Throws<ArgumentException>(() => scheduler.ScheduleAndCalculate(100m, packages, invalidVehicle));
        }
    }
}