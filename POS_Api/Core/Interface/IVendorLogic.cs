using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface IVendorLogic
    {
        public bool AddVendor(VendorModel model, string userId, string locationId);
        public bool AddVendorProductRelation(string uid, string productId, string locationId, string userId);

        public List<VendorModel> GetVendorByLocationId(string userId, string locationId);

        public bool UpdateVendor(VendorModel model, string userId, string locationId);
    }
}
