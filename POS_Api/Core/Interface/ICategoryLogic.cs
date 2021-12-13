using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface ICategoryLogic
    {
        public bool AddCategory(CategoryModel model, string userId, string locationId);
        public bool AddCategoryProductRelation(string uid, string productId, string locationId, string userId);
   
        public List<CategoryModel> GetCategoryByLocationId(string userId, string locationId);

        public bool UpdateCategory(CategoryModel model, string userId, string locationId);
    }
}
