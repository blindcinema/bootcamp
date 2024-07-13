using System;
using System.Collections.Generic;
namespace TrackIt.Models
{
    public class Package
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid SenderId { get; set; }
        public Guid? CourierId { get; set; } = Guid.Empty;
        public float Weight { get; set; }
        public string Remark { get; set; }
        public string DeliveryAddress { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public string DeliveryLog { get; set; }
        public List<Rating> Ratings { get; set; } = new List<Rating>();
        public string TrackingNumber { get; set; }
    }
}