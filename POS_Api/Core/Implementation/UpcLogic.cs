using POS_Api.Core.Interface;
using POS_Api.Model;
using POS_Api.Model.ReponseViewModel;
using POS_Api.Repository.Implementation;
using POS_Api.Repository.Interface;
using POS_Api.Shared.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace POS_Api.Core.Implementation
{
    public class UpcLogic : BaseHelper, IUpcLogic
    {
        private readonly IProductRepos _productRepos;
        private readonly IUserRepos _userRepos;
        private readonly ILocationRepos _locationRepos;

        private readonly IUpcRepos _upcRepos;
        public UpcLogic()
        {
            _userRepos = new UserRepos();
            _productRepos = new ProductRepos();
            _locationRepos = new LocationRepos();

            _upcRepos = new UpcRepos();


        }


        public UpcModel GetByUpc(string userId, string productUid, string locationUid, string upc)
        {
            bool isUserValid = _userRepos.VerifyUser(userId);
            bool isLocationValid = _locationRepos.VerifyUIdExist(locationUid);
            bool isProductValid = _productRepos.VerifyUIdExist(productUid);

            if (!isProductValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Product"));
            }

            if (!isUserValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }

            if (!isLocationValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Location"));
            }
            return _upcRepos.GetUpcById(productUid, locationUid, upc);
        }

        public bool VerifyUpc(string userId, string productUid, string locationUid, string upc)
        {
            bool isUserValid = _userRepos.VerifyUser(userId);
            bool isLocationValid = _locationRepos.VerifyUIdExist(locationUid);
            bool isProductValid = _productRepos.VerifyUIdExist(productUid);

            if (!isProductValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Product"));
            }

            if (!isUserValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }

            if (!isLocationValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Location"));
            }

            return _upcRepos.VerifyUpcExist(productUid, locationUid, upc);
        }

        public UpcPaginationModelVm GetUpcPagination(string userId, string productUid, string locationUid, int limit, int offset, string order)
        {
            bool isUserValid = _userRepos.VerifyUser(userId);
            bool isLocationValid = _locationRepos.VerifyUIdExist(locationUid);
            bool isProductValid = _productRepos.VerifyUIdExist(productUid);

            if (!isProductValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Product"));
            }

            if (!isUserValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }

            if (!isLocationValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Location"));
            }

            return _upcRepos.GetUpcPagination(productUid, locationUid, limit, offset, order);
        }

        public bool AddUpc(string productUid, string locationUid, string upc, string userId)
        {
            bool isUserValid = _userRepos.VerifyUser(userId);
            bool isLocationValid = _locationRepos.VerifyUIdExist(locationUid);
            bool isProductValid = _productRepos.VerifyUIdExist(productUid);

            if (!isProductValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Product"));
            }

            if (!isUserValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }

            if (!isLocationValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Location"));
            }

            return _upcRepos.AddUpcExecution(productUid, locationUid, upc, userId);
        }

        public bool RemoveUpc(string productUid, string locationUid, string upc, string userId)
        {
            bool isUserValid = _userRepos.VerifyUser(userId);
            bool isLocationValid = _locationRepos.VerifyUIdExist(locationUid);
            bool isProductValid = _productRepos.VerifyUIdExist(productUid);

            if (!isProductValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Product"));
            }

            if (!isUserValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }

            if (!isLocationValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Location"));
            }

            return _upcRepos.RemoveUpcExecution(productUid, locationUid, upc);
        }
    }
}
