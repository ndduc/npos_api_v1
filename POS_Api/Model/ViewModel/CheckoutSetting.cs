using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Model.ViewModel
{
    public class CheckoutSetting
    {
        public List<DiscountModel> DiscountList { get; set; }
        public List<TaxModel> TaxList { get; set; }
    }
}
