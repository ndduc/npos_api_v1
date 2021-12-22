using POS_Api.Model;
using System.Collections.Generic;

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

        public bool AddSectionExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId);
    }
}
