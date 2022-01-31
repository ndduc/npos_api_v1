using POS_Api.Model;
using POS_Api.Model.ReponseViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Repository.Interface
{
    public interface IItemCodeRepos
    {
        public bool RemoveItemCodeExecution(string productUid, string locationUid, string itemCode);
        public bool AddItemCodeExecution(string productUid, string locationUid, string upc, string itemCode);
        public bool VerifyItemCodeExist(string productUid, string locationUid, string itemCode);
        public ItemCodeModel GetItemCodeById(string productUid, string locationUid, string itemCode);
        public IEnumerable<ItemCodeModel> GetAllItemCodeByLocationAndProduct(string productUid, string locationUid);
        public ItemCodePaginationModelVm GetItemCodePagination(string productUid, string locationUid, int limit, int offset, string order);
    }
}
