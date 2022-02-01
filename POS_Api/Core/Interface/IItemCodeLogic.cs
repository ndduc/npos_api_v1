using POS_Api.Model;
using POS_Api.Model.ReponseViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface IItemCodeLogic
    {
        public ItemCodeModel GetByItemCode(string userId, string productUid, string locationUid, string itemCode);
        public bool VerifyItemCode(string userId, string productUid, string locationUid, string itemCode);
        public ItemCodePaginationModelVm GetItemCodePagination(string userId, string productUid, string locationUid, int limit, int offset, string order);
        public bool AddItemCode(string productUid, string locationUid, string itemCode, string userId);

        public bool RemoveItemCode(string productUid, string locationUid, string itemCode, string userId);
    }
}
