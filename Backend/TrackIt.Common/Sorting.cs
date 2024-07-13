using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackIt.Common
{
    public class Sorting
    {
        public string orderBy;
        public string sortOrder; 


        public Sorting(string orderBy, string sortOrder) { 
            this.orderBy = orderBy;
            this.sortOrder = sortOrder;
        }
    }
}
