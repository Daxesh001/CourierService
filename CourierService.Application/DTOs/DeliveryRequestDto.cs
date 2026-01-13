namespace CourierService.Application.DTOs
{
    public class DeliveryRequestDto
    {
        public decimal BaseDeliveryCost { get; set; }
        public List<PackageRequestDto> Packages { get; set; } = new();
        public VehicleRequestDto? Vehicles { get; set; }
    }
}
