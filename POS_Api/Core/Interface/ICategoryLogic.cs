using POS_Api.Model;
using System.Collections.Generic;

namespace POS_Api.Core.Interface
{
    public interface ICategoryLogic
    {
        public bool AddCategory(CategoryModel model, string userId, string locationId);
        public bool AddCategoryProductRelation(string uid, string productId, string locationId, string userId);

        public List<CategoryModel> GetCategoryByLocationId(string userId, string locationId);

        public bool UpdateCategory(CategoryModel model, string userId, string locationId);

        public int GetCategoryPaginateCount(Dictionary<string, string> param);
        public IEnumerable<CategoryModel> GetCategoryPaginate(Dictionary<string, string> param);

        public CategoryModel GetCategoryById(string userId, string locId, string CategoryId);

        public IEnumerable<CategoryModel> GetCategoryByDescription(string userId, string locId, string description);
    }
}
