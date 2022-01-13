using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Model
{
    public class UpcModel
    {
        public string Id { get; set; }
        public string ProductUid { get; set; }
        public string LocationUid { get; set; }
        public string Upc { get; set; }
        public string AddedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
