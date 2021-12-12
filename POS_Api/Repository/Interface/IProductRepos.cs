using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Repository.Interface
{
    public interface IProductRepos
    {
        public bool UpdateProductExecutionRelation(ProductModel model, string locationId);

        public IEnumerable<ProductModel> GetProductByLocationExecution(string locId);

        public List<string> GetProductItemCode(string locationId, string productId);

        public bool VerifyUIdUnique(string uid);

        public ProductModel GetProductByIdExecution(string locationId, string where);


        public bool AddProductExecution(ProductModel model);

        public bool AddProductExecutionRelation(ProductModel model, string userId, string locationId);

        public bool UpdateProductExecution(ProductModel model);

        public bool VerifyUIdExist(string uid);

    }
}
