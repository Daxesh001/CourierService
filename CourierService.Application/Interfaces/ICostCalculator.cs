namespace CourierService.Application.Interfaces
{
    public interface ICostCalculator
    {
        decimal CalculateDeliveryCost(decimal baseCost, decimal weightKg, decimal distanceKm);
    }
}
