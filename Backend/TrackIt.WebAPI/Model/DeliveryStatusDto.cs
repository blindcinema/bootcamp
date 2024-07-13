
    public class DeliveryStatusDto
    {
        public Guid Id { get; set; }
        public string? Status { get; set; }
        public DateTime? date { get; set; }
        public bool? IsActive { get; set; }
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        Guid? CreatedBy { get; set; }
        Guid? UpdatedBy { get; set; }

    }

