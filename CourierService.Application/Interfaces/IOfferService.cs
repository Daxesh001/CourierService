using CourierService.Domain.Entities;

namespace CourierService.Application.Interfaces
{
    public interface IOfferService
    {
        decimal GetDiscountPercent(string? code, Package package);
    }
}
