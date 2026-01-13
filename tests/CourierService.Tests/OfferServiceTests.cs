using CourierService.Application.Services;
using CourierService.Domain.Entities;
using Xunit;

namespace CourierService.Tests
{
    public class OfferServiceTests
    {
        [Fact]
        public void GetDiscountPercent_ReturnsDiscount_WhenOfferApplies()
        {
            var svc = new OfferService();
            var pkg = new Package { Id = "PKG1", WeightKg = 100m, DistanceKm = 100m };
            var discount = svc.GetDiscountPercent("OFR001", pkg);
            // OFR001: MinWeight 70-200, Distance 0-199 -> package qualifies
            Assert.Equal(10m, discount);
        }

        [Fact]
        public void GetDiscountPercent_ReturnsZero_WhenCodeInvalidOrDoesNotQualify()
        {
            var svc = new OfferService();
            var pkg = new Package { Id = "PKG1", WeightKg = 10m, DistanceKm = 10m };
            Assert.Equal(0m, svc.GetDiscountPercent(null, pkg));
            Assert.Equal(0m, svc.GetDiscountPercent("INVALID", pkg));
        }

        [Theory]
        [InlineData("OFR001", 70, 0, 10)]
        [InlineData("OFR001", 200, 199, 10)]
        [InlineData("OFR002", 100, 50, 7)]
        [InlineData("OFR002", 250, 150, 7)]
        [InlineData("OFR003", 10, 50, 5)]
        [InlineData("OFR003", 150, 250, 5)]
        public void GetDiscountPercent_BoundaryValues_ReturnsExpected(string code, decimal weight, decimal distance, decimal expected)
        {
            var svc = new OfferService();
            var pkg = new Package { Id = "PKG", WeightKg = weight, DistanceKm = distance };
            Assert.Equal(expected, svc.GetDiscountPercent(code, pkg));
        }
    }
} 