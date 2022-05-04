using POS_Api.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface ICheckoutSettingLogic
    {
        public CheckoutSetting GetCheckoutSetting(string userId, string locationId);
    }
}
