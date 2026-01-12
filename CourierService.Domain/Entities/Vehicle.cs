using System;

namespace CourierService.Domain.Entities
{
    public class Vehicle
    {
        public int NumberOfVehicles { get; set; }
        public decimal MaxSpeedKmH { get; set; }
        public decimal MaxCarryWeightKg { get; set; }
    }
}
