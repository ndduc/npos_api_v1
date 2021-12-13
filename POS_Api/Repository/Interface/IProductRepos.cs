using MySql.Data.MySqlClient;
using POS_Api.Core.Interface;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
using POS_Api.Model.ViewModel;
using POS_Api.Repository.Interface;
using POS_Api.Shared.DbHelper;
using POS_Api.Shared.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
namespace POS_Api.Repository.Interface
{
    public interface IProductRepos
    {
        public IEnumerable<ProductModel> GetProductByLocationExecution(string locId);
        public ProductModel GetProductByIdExecution(string locationId, string where);
        public List<string> GetProductItemCode(string locationId, string productId);
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);
        public bool AddProductExecution(ProductModel model);
        public bool AddProductExecutionRelation(ProductModel model, string userId, string locationId);
        public bool UpdateProductExecution(ProductModel model);
        public bool UpdateProductExecutionRelation(ProductModel model, string locationId);
        public ProductModelVm GetProductByIdWithMapExecution(string userId, string locationId, Dictionary<string, string> param);
    }
}
