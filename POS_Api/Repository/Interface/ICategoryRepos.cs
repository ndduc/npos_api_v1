using POS_Api.Model;
using System.Collections.Generic;

namespace POS_Api.Repository.Interface
{
    public interface ICategoryRepos
    {
        public bool UpdateCategoryExecution(CategoryModel model);
        public List<CategoryModel> GetCategoryByLocationIdExecution(string locationId);
        public bool AddCategoryExecution(CategoryModel model);
        public bool AddCategoryProductRelationExecution(string uid, string productId, string locationId, string userId);
        public bool VerifyCategoryProductRelationExist(string uid, string productId, string locationId);
        public bool VerifyCategoryProductRelationExist(string productId, string locationId);
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);

        public bool AddCategoryExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId);

        public bool UpsertCategoryExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId);

        public int GetCategoryPaginateCount(string locId);
        
        public IEnumerable<CategoryModel> GetCategoryPaginateByDefault(string locId, int startIdx, int endIdx);

        public CategoryModel GetCategoryById(string locId, string CategoryId);

        public IEnumerable<CategoryModel> GetCategoryByDescription(string locId, string description);
    }
}
