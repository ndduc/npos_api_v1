using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Repository.Interface
{
    public interface IItemCodeRepos
    {
        public bool RemoveItemCodeExecution(string productUid, string locationUid, string upc);
        public bool AddItemCodeExecution(string productUid, string locationUid, string upc, string itemCode);
        public bool VerifyItemCodeExist(string productUid, string locationUid, string upc);
        public ItemCodeModel GetItemCodeById(string productUid, string locationUid, string upc);
        public IEnumerable<ItemCodeModel> GetAllItemCodeByLocationAndProduct(string productUid, string locationUid);
    }
}
