using CourierService.Application.DTOs;
using CourierService.Domain.Entities;

namespace CourierService.Application.Interfaces
{
    public interface IDeliveryScheduler
    {
        DeliveryResultDto ScheduleAndCalculate(decimal baseCost, List<Package> packages, Vehicle vehicle);
    }
}
