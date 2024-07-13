using System;
using System.Collections.Generic;
namespace TrackIt.Models
{
    public class DeliveryLog
    {
        public Guid Id { get; set; }
        public Guid PackageId { get; set; }
        public Guid DeliveryStatusId { get; set; }
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        Guid? CreatedBy { get; set; }
        Guid? UpdatedBy { get; set; }
        public List<DeliveryStatus> DeliveryStatuses { get; set; } = new List<DeliveryStatus>();
    }
}
