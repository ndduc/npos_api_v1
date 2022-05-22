using POS_Api.Model;
using System.Collections.Generic;

namespace POS_Api.Repository.Interface
{
    public interface ISubCategoryRepos
    {
        public bool UpdateSubCategoryExecution(SubCategoryModel model);
        public List<SubCategoryModel> GetSubCategoryByLocationIdExecution(string locationId);
        public bool AddSubCategoryExecution(SubCategoryModel model);
        public bool AddSubCategoryProductRelationExecution(string uid, string productId, string locationId, string userId);
        public bool VerifySubCategoryProductRelationExist(string uid, string productId, string locationId);
        public bool VerifySubCategoryProductRelationExist(string productId, string locationId);
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);

        public bool AddSubCategoryExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId);

        public bool UpsertSubCategoryExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId);

        public int GetSubCategoryPaginateCount(string locId);
        
        public IEnumerable<SubCategoryModel> GetSubCategoryPaginateByDefault(string locId, int startIdx, int endIdx);

        public SubCategoryModel GetSubCategoryById(string locId, string CategoryId);

        public IEnumerable<SubCategoryModel> GetSubCategoryByDescription(string locId, string description);

        public IEnumerable<SubCategoryModel> GetSubCategoryByDepartmentId(string locId, string deptId);
    }
}
