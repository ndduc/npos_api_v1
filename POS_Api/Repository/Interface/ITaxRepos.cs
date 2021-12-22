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
    }
}
