using System;
namespace TrackIt.Models
{
    public class PaymentMethod
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
