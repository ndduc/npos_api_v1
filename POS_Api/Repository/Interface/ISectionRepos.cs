using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Repository.Interface
{
    public  interface ISectionRepos
    {
        public bool UpdateSectionExecution(SectionModel model);
        public List<SectionModel> GetSectionByLocationIdExecution(string locationId);
        public bool AddSectionExecution(SectionModel model);
        public bool AddSectionProductRelationExecution(string uid, string productId, string locationId, string userId);
        public bool VerifySectionProductRelationExist(string uid, string productId, string locationId);
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);
    }
}
