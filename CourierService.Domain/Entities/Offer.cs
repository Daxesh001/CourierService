namespace CourierService.Domain.Entities
{
    public class Offer
    {
        public string Code { get; set; } = string.Empty;
        public int DiscountPercent { get; set; }
        public decimal MinWeightKg { get; set; }
        public decimal MaxWeightKg { get; set; }
        public decimal MinDistanceKm { get; set; }
        public decimal MaxDistanceKm { get; set; }
    }
}
