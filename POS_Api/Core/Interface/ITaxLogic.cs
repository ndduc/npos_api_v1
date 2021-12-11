using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface ITaxLogic
    {
        public bool AddTax(TaxModel model, string userId, string locationId);
        public bool VerifyUIdExist(string uid);

        public List<TaxModel> GetTaxByLocationId(string userId, string locationId);
    }
}
