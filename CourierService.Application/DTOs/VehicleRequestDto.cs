namespace CourierService.Application.DTOs
{
    public class VehicleRequestDto
    {
        public int NumberOfVehicles { get; set; }
        public decimal MaxSpeedKmH { get; set; }
        public decimal MaxCarryWeightKg { get; set; }
    }
}