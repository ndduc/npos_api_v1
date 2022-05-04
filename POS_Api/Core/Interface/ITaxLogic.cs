using POS_Api.Model;
using System.Collections.Generic;

namespace POS_Api.Core.Interface
{
    public interface ITaxLogic
    {
        public bool AddTax(TaxModel model, string userId, string locationId);

        public List<TaxModel> GetTaxByLocationId(string userId, string locationId);

        public bool AddTaxProductRelation(string productId, string locationId, string taxId, string userId);

        public bool UpdateTax(TaxModel model, string userId, string locationId);

        public int GetTaxPaginateCount(Dictionary<string, string> param);
        public IEnumerable<TaxModel> GetTaxPaginate(Dictionary<string, string> param);

        public TaxModel GetTaxById(string userId, string locId, string TaxId);

        public IEnumerable<TaxModel> GetTaxByDescription(string userId, string locId, string description);

    }
}
