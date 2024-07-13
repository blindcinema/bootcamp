namespace TrackIt.WebAPI.Model
{
    public class CreatePackageRequest
    {
        public Guid? CourierId { get; set; }
        public Guid ClientId { get; set; }
        public string Status { get; set; } = "Pending";
        public Guid SenderId { get; set; }
        public float Weight { get; set; }
        public string? Remark { get; set; }
        public string? DeliveryAddress { get; set; }
        public Guid CreatedBy { get; set; } = Guid.Empty;
        public Guid UpdatedBy { get; set; } = Guid.Empty;
    }
}
