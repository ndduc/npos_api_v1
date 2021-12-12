using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface IDiscountLogic
    {
        public bool AddDiscount(DiscountModel model, string userId, string locationId);
        public bool VerifyUIdExist(string uid);

        public List<DiscountModel> GetDiscountByLocationId(string userId, string locationId);

        public bool AddDiscountProductRelation(string productId, string locationId, string discountId, string userId);

        public bool UpdateDiscount(DiscountModel model, string userId, string locationId);
    }
}
