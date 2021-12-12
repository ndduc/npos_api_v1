using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface ILocationProductRelationLogic
    {
        public bool IsRelationLocationProductExist(string locationId, string productId);
        public bool IsRelationItemCodeExist(string locationId, string productId, string itemCode);
        public bool AddRelationLocationProduct(string locationId, string productId, string userId);
        public bool AddRelationItemCode(string locationId, string productId, string userId, string itemCode);
        public bool DeleteRelationItemCode(string locationId, string productId, string itemCode);
        public bool UpdateRelationItemCode(string locationId, string productId, string userId, string newItemCode, string oldItemCode);

        public bool IsProductLocationExist(string locationId, string productId);
    }
}
