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
    public interface ISectionRepos
    {
        public bool AddSectionProductRelationExecution(string uid, string productId, string locationId, string userId);
        public bool VerifySectionProductRelationExist(string uid, string productId, string locationId);
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);
        public bool AddSectionExecution(SectionModel model);
        public List<SectionModel> GetSectionByLocationIdExecution(string locationId);
        public bool UpdateSectionExecution(SectionModel model);
    }
}
