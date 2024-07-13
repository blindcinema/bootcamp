using System;
using System.Collections.Generic;
namespace TrackIt.Models
{
    public class Courier
    {
        public Guid Id { get; set; }
        public string Surname { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public List<Package> Packages { get; set; } = new List<Package>();
        public Guid? UserId { get; set; }
    }
}