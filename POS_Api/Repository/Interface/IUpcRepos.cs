using POS_Api.Model;
using POS_Api.Model.ReponseViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Repository.Interface
{
    public interface IUpcRepos
    {
        public bool RemoveUpcExecution(string productUid, string locationUid, string upc);
        public bool AddUpcExecution(string productUid, string locationUid, string upc, string userId);
        public bool VerifyUpcExist(string productUid, string locationUid, string upc);
        public UpcModel GetUpcById(string productUid, string locationUid, string upc);
        public IEnumerable<UpcModel> GetAllUpcByLocationAndProduct(string productUid, string locationUid);

        public UpcPaginationModelVm GetUpcPagination(string productUid, string locationUid, int limit, int offset, string order);
    }
}
