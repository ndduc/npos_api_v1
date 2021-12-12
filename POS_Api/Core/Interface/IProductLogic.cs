using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface IProductLogic
    {
        public bool AddProduct(ProductModel model, string userId, string locationId);
        public bool UpdateProduct(ProductModel model, string userId, string locationId);
        public IEnumerable<ProductModel> GetProductByLocation(string userId, string locationId);

        public ProductModel GetProductById(string userId, string locationId, Dictionary<string, string> param);

        public bool VerifyUIdExist(string uid);
    }
}
