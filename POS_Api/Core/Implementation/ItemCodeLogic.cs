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
    public class ItemCodeLogic : BaseHelper
    {
        private readonly IProductRepos _productRepos;
        private readonly IUserRepos _userRepos;
        private readonly ILocationRepos _locationRepos;

        private readonly IItemCodeRepos _itemCodeRepos;

        public ItemCodeLogic()
        {
            _userRepos = new UserRepos();
            _productRepos = new ProductRepos();
            _locationRepos = new LocationRepos();

            _itemCodeRepos = new ItemCodeRepos();
        }

        public bool VerifyItemCode(string userId, string productUid, string locationUid, string itemCode)
        {
            return false;
        }

        public ItemCodePaginationModelVm GetItemCodePagination(string userId, string productUid, string locationUid, int limit, int offset, string order)
        {
            bool isUserValid = _userRepos.VerifyUser(userId);
            bool isLocationValid = _locationRepos.VerifyUIdExist(locationUid);

            if (!isUserValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "unauthorized access"));
            }

            if (!isLocationValid)
            {
                throw GenericException(GenerateExceptionMessage(GetType().Name, MethodBase.GetCurrentMethod().Name, "Invalid Location"));
            }

            return _itemCodeRepos.GetItemCodePagination(productUid, locationUid, limit, offset, order);
        }
    }


}
