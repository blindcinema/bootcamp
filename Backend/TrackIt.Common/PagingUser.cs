using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackIt.Models;

namespace TrackIt.Common
{
    public class PagingUser
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? TotalCount { get; set; }
        public List<User> Users { get; set; } = new List<User>();
    }
}

