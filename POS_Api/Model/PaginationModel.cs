using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Model
{
    public class PaginationModel
    {
        public int Limit { get; set; }
        public int OffSet { get; set; }
        public int Count { get; set; }
        public string Order { get; set; }
    }
}
