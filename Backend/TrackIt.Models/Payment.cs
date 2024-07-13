using System;
namespace TrackIt.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid PackageId { get; set; }
        public int Price { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
