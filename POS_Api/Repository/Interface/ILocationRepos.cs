using MySql.Data.MySqlClient;
using POS_Api.Core.Interface;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
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
    public interface ILocationRepos
    {
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);
        public bool AddLocationExecution(LocationModel model);
        public IEnumerable<LocationModel> GetLocationByUserIdExecution(string userId);
    }
}
