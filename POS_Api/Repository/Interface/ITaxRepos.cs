using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Repository.Interface
{
    public interface ITaxRepos
    {
        public bool UpdateTaxExecution(TaxModel model);
        public bool AddTaxProductRelationExecution(string productId, string locationId, string taxId, string userId);
        public List<TaxModel> GetTaxByLocationIdExecution(string locationId);
        public bool AddTaxExecution(TaxModel model);
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);
        public bool VerifyTaxProductRelation(string productId, string locationId);
    }
}
