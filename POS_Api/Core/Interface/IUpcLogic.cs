using POS_Api.Model;
using POS_Api.Model.ReponseViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface IUpcLogic
    {
        public UpcModel GetByUpc(string userId, string productUid, string locationUid, string upc);
        public bool VerifyUpc(string userId, string productUid, string locationUid, string upc);
        public UpcPaginationModelVm GetUpcPagination(string userId, string productUid, string locationUid, int limit, int offset, string order);
        public bool AddUpc(string productUid, string locationUid, string upc, string userId);

        public bool RemoveUpc(string productUid, string locationUid, string upc, string userId);
    }
}
