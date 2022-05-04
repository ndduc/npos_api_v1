using POS_Api.Core.Interface;
using POS_Api.Model.ViewModel;
using POS_Api.Repository.Implementation;
using POS_Api.Repository.Interface;
using POS_Api.Shared.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Implementation
{
    public class CheckoutSettingLogic : BaseHelper, ICheckoutSettingLogic
    {
        private readonly IDiscountLogic _discountLogic;
        private readonly ITaxLogic _taxLogic;

        public CheckoutSettingLogic() {
            _discountLogic = new DiscountLogic();
            _taxLogic = new TaxLogic();
        }

        public CheckoutSetting GetCheckoutSetting(string userId, string locationId){

            var discounts = _discountLogic.GetDiscountByLocationId(userId, locationId);
            var taxes = _taxLogic.GetTaxByLocationId(userId, locationId);

            return new CheckoutSetting()
            {
                DiscountList = discounts,
                TaxList = taxes
            };
        }
    }
}
