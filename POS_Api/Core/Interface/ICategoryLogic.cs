using POS_Api.Model;
using System.Collections.Generic;

namespace POS_Api.Core.Interface
{
    public interface ISubCategoryLogic
    {
        public bool AddSubCategory(SubCategoryModel model, string userId, string locationId);
        public bool AddSubCategoryProductRelation(string uid, string productId, string locationId, string userId);

        public List<SubCategoryModel> GetSubCategoryByLocationId(string userId, string locationId);

        public bool UpdateSubCategory(SubCategoryModel model, string userId, string locationId);

        public int GetSubCategoryPaginateCount(Dictionary<string, string> param);
        public IEnumerable<SubCategoryModel> GetSubCategoryPaginate(Dictionary<string, string> param);

        public SubCategoryModel GetSubCategoryById(string userId, string locId, string SubCategoryId);

        public IEnumerable<SubCategoryModel> GetSubCategoryByDescription(string userId, string locId, string description);

        public IEnumerable<SubCategoryModel> GetSubCategoryByDepartmentId(string userId, string locId, string deptId);
    }
}
