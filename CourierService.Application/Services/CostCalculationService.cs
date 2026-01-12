using CourierService.Application.Interfaces;

namespace CourierService.Application.Services
{
    public class CostCalculationService : ICostCalculator
    {
        private const decimal PerKg = 10m;
        private const decimal PerKm = 5m;

        public decimal CalculateDeliveryCost(decimal baseCost, decimal weightKg, decimal distanceKm)
        {
            return baseCost + (weightKg * PerKg) + (distanceKm * PerKm);
        }
    }
}
