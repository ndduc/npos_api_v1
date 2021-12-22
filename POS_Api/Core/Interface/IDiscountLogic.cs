﻿using POS_Api.Model;
using System.Collections.Generic;

namespace POS_Api.Core.Interface
{
    public interface IDiscountLogic
    {
        public bool AddDiscount(DiscountModel model, string userId, string locationId);

        public List<DiscountModel> GetDiscountByLocationId(string userId, string locationId);

        public bool AddDiscountProductRelation(string productId, string locationId, string discountId, string userId);

        public bool UpdateDiscount(DiscountModel model, string userId, string locationId);
    }
}
