using MySql.Data.MySqlClient;
using POS_Api.Core.Interface;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
using POS_Api.Model.EnumData;
using POS_Api.Model.ViewModel;
using POS_Api.Repository.Implementation;
using POS_Api.Repository.Interface;
using POS_Api.Shared.DbHelper;
using POS_Api.Shared.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace POS_Api.Core.Implementation
{
    public class ProductLogic : BaseHelper, IProductLogic
    {
        private readonly IProductRepos _productRepos;
        private readonly IUserRepos _userRepos;
        public ProductLogic()
        {
            _userRepos = new UserRepos();
            _productRepos = new ProductRepos();
        }

        public bool AddProduct(ProductModel model, string userId, string locationId)
        {
            string id = null;
            bool isUnqiue = false;
            bool isUserValid = false;
            bool isSucess = false;
            bool isRelationSucess = false;
            while (!isUnqiue)
            {
                id = Guid.NewGuid().ToString();
                isUnqiue = _productRepos.VerifyUIdUnique(id);
            }

            isUserValid = _userRepos.VerifyUser(userId);

            if(isUserValid)
            {
                model.UId = id;
                model.AddedBy = userId;

                isSucess = _productRepos.AddProductExecution(model);

                if (isSucess)
                {
                    return _productRepos.AddProductExecutionRelation(model, userId, locationId);
                } else
                {
                    return isSucess;
                }
            } else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }

        }

        public bool UpdateProduct(ProductModel model, string userId, string locationId)
        {
            bool isUnqiue = false;
            bool isUserValid = false;
            bool isSucess = false;
            bool isRelationSucess = false;
            isUserValid = _userRepos.VerifyUser(userId);

            if (isUserValid)
            {
                model.UpdatedBy = userId;

                isSucess = _productRepos.UpdateProductExecution(model);

                if (isSucess)
                {
                    int retry = 0;
                    while (!isRelationSucess && retry > 5)
                    {
                        retry++;
                        isRelationSucess = _productRepos.UpdateProductExecutionRelation(model, locationId);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }

        public IEnumerable<ProductModel> GetProductByLocation(string userId, string locationId)
        {
            if (_userRepos.VerifyUser(userId))
            {
                return _productRepos.GetProductByLocationExecution(locationId);
            }
            else
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }
        }


        public ProductModelVm GetProductById(string userId, string locationId, Dictionary<string, string> param)
        {
            return _productRepos.GetProductByIdWithMapExecution(userId, locationId, param);
        }

        public bool AddRelationItemCode(string locationId, string productId, string userId, string itemCode)
        {
            return _productRepos.AddRelationItemCode(locationId, productId, userId, itemCode);
        }
    }
}
