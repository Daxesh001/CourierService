using CourierService.Application.DTOs;
using CourierService.Application.Interfaces;
using CourierService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CourierService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController(IDeliveryScheduler _scheduler) : ControllerBase
    {
        [HttpPost]
        public ActionResult<DeliveryResultDto> Post([FromBody] DeliveryRequestDto request)
        {
            if (request == null || request.Packages == null || !request.Packages.Any()) 
                return BadRequest("Invalid request");

            var packages = request.Packages.Select(p => new Package
            {
                Id = p.Id,
                WeightKg = p.WeightKg,
                DistanceKm = p.DistanceKm,
                OfferCode = p.OfferCode
            }).ToList();

            var vehicle = request.Vehicles == null ? new Vehicle { NumberOfVehicles = 0 } : new Vehicle
            {
                NumberOfVehicles = request.Vehicles.NumberOfVehicles,
                MaxSpeedKmH = request.Vehicles.MaxSpeedKmH,
                MaxCarryWeightKg = request.Vehicles.MaxCarryWeightKg
            };

            var result = _scheduler.ScheduleAndCalculate(request.BaseDeliveryCost, packages, vehicle);

            return Ok(result);
        }
    }
}
