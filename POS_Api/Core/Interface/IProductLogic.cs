using POS_Api.Model;
using POS_Api.Model.ReponseViewModel;
using POS_Api.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface IProductLogic
    {
        public bool AddProduct(ProductModel model, string userId, string locationId);
         
        public ProductAddModelVm AddProduct(ProductModel model, Dictionary<string, string> param);

        public bool UpdateProduct(ProductModel model, string userId, string locationId);
        public IEnumerable<ProductModel> GetProductByLocation(string userId, string locationId);

        public ProductModelVm GetProductById(string userId, string locationId, Dictionary<string, string> param);
        public bool AddRelationItemCode(string locationId, string productId, string userId, string itemCode);

        public int GetProductPaginateCount(Dictionary<string, string> param);
        public IEnumerable<ProductModel> GetProductPaginate(Dictionary<string, string> param);
    }
}
