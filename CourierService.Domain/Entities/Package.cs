using System;

namespace CourierService.Domain.Entities
{
    public class Package
    {
        public string Id { get; set; } = string.Empty;
        public decimal WeightKg { get; set; }
        public decimal DistanceKm { get; set; }
        public string? OfferCode { get; set; }
    }
}
