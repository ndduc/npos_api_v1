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

        public bool UpsertSectionExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId);


        public int GetSectionPaginateCount(string locId);

        public IEnumerable<SectionModel> GetSectionPaginateByDefault(string locId, int startIdx, int endIdx);

        public SectionModel GetSectionById(string locId, string SectionId);

        public IEnumerable<SectionModel> GetSectionByDescription(string locId, string description);
    }
}
