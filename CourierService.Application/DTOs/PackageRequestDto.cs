namespace CourierService.Application.DTOs
{
    public class PackageRequestDto
    {
        public string Id { get; set; } = string.Empty;
        public decimal WeightKg { get; set; }
        public decimal DistanceKm { get; set; }
        public string? OfferCode { get; set; }
    }
}