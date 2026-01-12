using CourierService.Application.DTOs;
using CourierService.Application.Interfaces;
using CourierService.Domain.Entities;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace CourierService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryScheduler _scheduler;

        public DeliveryController(IDeliveryScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        [HttpPost]
        public ActionResult<DeliveryResultDto> Post([FromBody] DeliveryRequestDto request)
        {
            if (request == null || request.Packages == null || !request.Packages.Any()) return BadRequest("Invalid request");

            var packages = request.Packages.Select(p => new Package
            {
                Id = p.Id,
                WeightKg = p.WeightKg,
                DistanceKm = p.DistanceKm,
                OfferCode = p.OfferCode
            }).ToList();

            var vehicle = request.Vehicles == null ? null : new Vehicle
            {
                NumberOfVehicles = request.Vehicles.NumberOfVehicles,
                MaxSpeedKmH = request.Vehicles.MaxSpeedKmH,
                MaxCarryWeightKg = request.Vehicles.MaxCarryWeightKg
            };

            var result = _scheduler.ScheduleAndCalculate(request.BaseDeliveryCost, packages, vehicle ?? new Vehicle { NumberOfVehicles = 0 });

            return Ok(result);
        }

        [HttpGet("sample")]
        public ActionResult<DeliveryResultDto> Sample()
        {
            var request = new DeliveryRequestDto
            {
                BaseDeliveryCost = 100m,
                Packages = new List<PackageRequestDto>
                {
                    new PackageRequestDto { Id = "PKG1", WeightKg = 50, DistanceKm = 30, OfferCode = "OFR001" },
                    new PackageRequestDto { Id = "PKG2", WeightKg = 75, DistanceKm = 125, OfferCode = "OFR002" },
                    new PackageRequestDto { Id = "PKG3", WeightKg = 175, DistanceKm = 100, OfferCode = "OFR003" }
                },
                Vehicles = new VehicleRequestDto { NumberOfVehicles = 2, MaxCarryWeightKg = 200, MaxSpeedKmH = 70 }
            };

            var packages = request.Packages.Select(p => new Package
            {
                Id = p.Id,
                WeightKg = p.WeightKg,
                DistanceKm = p.DistanceKm,
                OfferCode = p.OfferCode
            }).ToList();

            var result = _scheduler.ScheduleAndCalculate(request.BaseDeliveryCost, packages, new Vehicle { NumberOfVehicles = 2, MaxCarryWeightKg = 200, MaxSpeedKmH = 70 });

            return Ok(result);
        }
    }
}
