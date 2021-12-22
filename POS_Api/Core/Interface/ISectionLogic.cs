using POS_Api.Model;
using System.Collections.Generic;

namespace POS_Api.Core.Interface
{
    public interface ISectionLogic
    {
        public bool AddSection(SectionModel model, string userId, string locationId);
        public bool AddSectionProductRelation(string uid, string productId, string locationId, string userId);
        public List<SectionModel> GetSectionByLocationId(string userId, string locationId);

        public bool UpdateSection(SectionModel model, string userId, string locationId);

        public int GetSectionPaginateCount(Dictionary<string, string> param);
        public IEnumerable<SectionModel> GetSectionPaginate(Dictionary<string, string> param);

        public SectionModel GetSectionById(string userId, string locId, string SectionId);

        public IEnumerable<SectionModel> GetSectionByDescription(string userId, string locId, string description);
    }
}
