using CourierService.Application.Services;
using Xunit;

namespace CourierService.Tests
{
    public class CostCalculationServiceTests
    {
        [Fact]
        public void CalculateDeliveryCost_GivenValues_ReturnsExpected()
        {
            var svc = new CostCalculationService();
            var result = svc.CalculateDeliveryCost(100m, 5m, 5m);
            Assert.Equal(100m + (5m * 10m) + (5m * 5m), result);
        }

        [Fact]
        public void CalculateDeliveryCost_ZeroWeightDistance_ReturnsBase()
        {
            var svc = new CostCalculationService();
            var result = svc.CalculateDeliveryCost(50m, 0m, 0m);
            Assert.Equal(50m, result);
        }
    }
} 