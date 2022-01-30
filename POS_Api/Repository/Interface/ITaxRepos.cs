using POS_Api.Model;
using System.Collections.Generic;

namespace POS_Api.Repository.Interface
{
    public interface ITaxRepos
    {
        public List<TaxModel> GetTaxByLocationIdExecution(string locationId);
        public bool AddTaxExecution(TaxModel model);
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);
        public bool VerifyTaxProductRelation(string productId, string locationId);
        public bool AddTaxProductRelationExecution(string productId, string locationId, string taxId, string userId);
        public bool UpdateTaxExecution(TaxModel model);

        public bool AddTaxExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId);

        public bool UpdateTaxExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId);

        public int GetTaxPaginateCount(string locId);

        public IEnumerable<TaxModel> GetTaxPaginateByDefault(string locId, int startIdx, int endIdx);

        public TaxModel GetTaxById(string locId, string TaxId);

        public IEnumerable<TaxModel> GetTaxByDescription(string locId, string description);
    }
}
