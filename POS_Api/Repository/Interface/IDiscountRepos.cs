using POS_Api.Model;
using System.Collections.Generic;

namespace POS_Api.Repository.Interface
{
    public interface IDiscountRepos
    {
        public bool UpdateDiscountExecution(DiscountModel model);
        public List<DiscountModel> GetDiscountByLocationIdExecution(string locationId);
        public bool AddDiscountExecution(DiscountModel model);
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);
        public bool VerifyDiscountProductRelation(string productId, string locationId);
        public bool AddDiscountProductRelationExecution(string productId, string locationId, string discountId, string userId);
        public bool AddDiscountExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId);
    }

}
