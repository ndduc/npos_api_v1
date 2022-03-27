using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Model.ReponseViewModel
{
    public class GenericPaginationModelVm<T>
    {
        public int Count { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        public IEnumerable<T> DataObject { get; set; }
    }
}
