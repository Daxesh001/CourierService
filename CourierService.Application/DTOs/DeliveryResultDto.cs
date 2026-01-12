namespace CourierService.Application.DTOs
{
    public class PackageResultDto
    {
        public string Id { get; set; } = string.Empty;
        public decimal DeliveryCost { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalCost { get; set; }
        public decimal ETAHours { get; set; }
    }

    public class DeliveryResultDto
    {
        public decimal TotalCost { get; set; }
        public List<PackageResultDto> Packages { get; set; } = new();
        public decimal MaxDeliveryTimeHours { get; set; }
    }
}
