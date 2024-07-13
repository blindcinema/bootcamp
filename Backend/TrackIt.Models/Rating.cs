using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackIt.Models
{
    public class Rating
    { public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid PackageId { get; set; }
    public int RatingNumber {  get; set; }
    public string Comment { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    }
}
