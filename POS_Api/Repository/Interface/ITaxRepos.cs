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
    public interface ITaxRepos
    {
        public List<TaxModel> GetTaxByLocationIdExecution(string locationId);
        public bool AddTaxExecution(TaxModel model);
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);
        public bool VerifyTaxProductRelation(string productId, string locationId);
        public bool AddTaxProductRelationExecution(string productId, string locationId, string taxId, string userId);
        public bool UpdateTaxExecution(TaxModel model);
    }
}
