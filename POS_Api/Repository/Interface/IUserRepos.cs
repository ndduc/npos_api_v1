using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Repository.Interface
{
    public interface IUserRepos
    {
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUserName(string userName);
        public string VerifyUserAuthority(string uid);
        public bool VerifyUser(string uid);
        public bool AddUserExecution(UserModel userModel);
        public UserModel GetUserByPassAndUserNameExecution(string userName, string pass);

        public bool UpdateUserExecution(UserModel userModel);
        public bool UpdatePasswordExecution(UserModel userModel);
    }
}
