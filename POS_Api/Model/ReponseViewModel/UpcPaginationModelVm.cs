using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Model.ReponseViewModel
{
    public class UpcPaginationModelVm
    {
        public List<UpcModel> UpcList { get; set; }
        public PaginationModel PaginationObject { get; set; }
    }
}
