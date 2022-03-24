using POS_Api.Model;
using POS_Api.Model.ViewModel;
using System.Collections.Generic;
namespace POS_Api.Repository.Interface
{
    public interface IProductRepos
    {
        public IEnumerable<ProductModel> GetProductByLocationExecution(string locId);
        public ProductModel GetProductByIdExecution(string locationId, string where);
        public List<string> GetProductItemCode(string locationId, string productId);
        public List<string> GetProductUpc(string locationId, string productId);
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);
        public bool AddProductExecution(ProductModel model);
        public bool AddProductExecutionRelation(ProductModel model, string userId, string locationId);
        public bool UpdateProductExecution(ProductModel model);
        public bool UpdateProductExecutionRelation(ProductModel model, string locationId);
        public ProductModelVm GetProductByIdWithMapExecution(string userId, string locationId, Dictionary<string, string> param);

        public bool IsRelationLocationProductExist(string locationId, string productId);
        public bool IsRelationItemCodeExist(string locationId, string productId, string itemCode);
        public bool AddRelationLocationProduct(string locationId, string productId, string userId);
        public bool AddRelationItemCode(string locationId, string productId, string userId, string itemCode);
        public bool DeleteRelationItemCode(string locationId, string productId, string itemCode);
        public bool UpdateRelationItemCode(string locationId, string productId, string userId, string newItemCode, string oldItemCode);

        public bool IsProductLocationExist(string locationId, string productId);

        public bool AddRelationUpc(string locationId, string productId, string userId, string upc);

        public int GetProductPaginateCount(string locId);
        public IEnumerable<ProductModel> GetProductPaginateByDefault(string locId, int startIdx, int endIdx);
    }
}
