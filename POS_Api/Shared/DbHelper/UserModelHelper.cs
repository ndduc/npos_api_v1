using MySql.Data.MySqlClient;
using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Shared.DbHelper
{
    public static class UserModelHelper
    {
        public static UserModel GetUserData(MySqlDataReader reader)
        {
            UserModel model;
            model = new UserModel(
            reader.GetString("uid"),
            reader.GetString("UserName"),
            reader.GetString("Password"),
            reader.GetString("FirstName"),
            reader.GetString("LastName"),
            reader.GetString("Email"),
            DbHelper.TryGet(reader, "Email2"),
            DbHelper.TryGet(reader, "Phone"),
            DbHelper.TryGet(reader, "Address"),
            DbHelper.TryGet(reader, "UserType"),
            DbHelper.TryGet(reader, "AddedDateTime"),
            DbHelper.TryGet(reader, "UpdatedDateTime")
            )
            {
                IsAuthorize = true
            };
            return model;
        }

        public static UserModel GetUserDataHaveError(bool isAuthorize, bool isError, string Error)
        {
            UserModel model;
            model = new UserModel
            {
                IsAuthorize = isAuthorize,
                IsError = isError,
                Error = Error
            };
            return model;
        }
    }
}
