using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackIt.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
        public Guid UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public List<Rating> Ratings { get; set; } = new List<Rating>();
        public List<Package> Packages { get; set; } = new List<Package>();
    }
}
