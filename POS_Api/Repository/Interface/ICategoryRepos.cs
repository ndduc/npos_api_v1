using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Repository.Interface
{
    public interface ICategoryRepos
    {
        public bool UpdateCategoryExecution(CategoryModel model);
        public List<CategoryModel> GetCategoryByLocationIdExecution(string locationId);
        public bool AddCategoryExecution(CategoryModel model);
        public bool AddCategoryProductRelationExecution(string uid, string productId, string locationId, string userId);
        public bool VerifyCategoryProductRelationExist(string uid, string productId, string locationId);
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);

        public bool AddCategoryExecutionFromList(List<string> itemIdlist, string productId, string locationId, string userId);
    }
}
