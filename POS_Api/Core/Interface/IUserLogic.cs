using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface IUserLogic
    {
        public UserModel GetUserByPassAndUserName(string userName, string pass);

        public bool AddUser(UserModel userModel);

        public bool UpdateUser(UserModel userModel);

        public bool UpdatePassword(UserModel userModel);

        public bool VerifyUserName(string userName);

        public string VerifyUserAuthority(string uid);

        public bool VerifyUIdUnique(string uid);

        public bool AddUserWithParent(UserModel userModel, string parentId, string locationId, string type);

        public bool VerifyUser(string uid);

    }
}
