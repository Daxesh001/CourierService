using CourierService.Application.Interfaces;
using CourierService.Domain.Entities;

namespace CourierService.Application.Services
{
    public class OfferService : IOfferService
    {
        private readonly List<Offer> _offers = new()
        {
            new Offer { Code = "OFR001", DiscountPercent = 10, MinWeightKg = 70, MaxWeightKg = 200, MinDistanceKm = 0, MaxDistanceKm = 199 },
            new Offer { Code = "OFR002", DiscountPercent = 7, MinWeightKg = 100, MaxWeightKg = 250, MinDistanceKm = 50, MaxDistanceKm = 150 },
            new Offer { Code = "OFR003", DiscountPercent = 5, MinWeightKg = 10, MaxWeightKg = 150, MinDistanceKm = 50, MaxDistanceKm = 250 }
        };

        public decimal GetDiscountPercent(string? code, Package package)
        {
            if (string.IsNullOrWhiteSpace(code)) return 0m;
            var offer = _offers.Find(o => o.Code.Equals(code, System.StringComparison.OrdinalIgnoreCase));
            if (offer == null) return 0m;

            if (package.WeightKg >= offer.MinWeightKg && package.WeightKg <= offer.MaxWeightKg
                && package.DistanceKm >= offer.MinDistanceKm && package.DistanceKm <= offer.MaxDistanceKm)
            {
                return offer.DiscountPercent;
            }

            return 0m;
        }
    }
}
