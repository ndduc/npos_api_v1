using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface ISectionLogic
    {
        public bool AddSection(SectionModel model, string userId, string locationId);
        public bool AddSectionProductRelation(string uid, string productId, string locationId, string userId);
        public bool VerifyUIdExist(string uid);

        public List<SectionModel> GetSectionByLocationId(string userId, string locationId);
    }
}
