﻿using POS_Api.Model;
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
    }
}