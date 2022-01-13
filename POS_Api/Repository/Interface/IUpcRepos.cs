using POS_Api.Model;
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
    }
}
