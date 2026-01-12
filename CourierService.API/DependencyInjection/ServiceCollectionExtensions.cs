using CourierService.Application.Interfaces;
using CourierService.Application.Services;

namespace CourierService.API.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICostCalculator, CostCalculationService>();
            services.AddScoped<IOfferService, OfferService>();
            services.AddScoped<IDeliveryScheduler, DeliverySchedulingService>();

            return services;
        }
    }
}
