using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface IDepartmentLogic
    {
        public bool AddDepartment(DepartmentModel model, string userId, string locationId);
        public bool AddDepartmentProductRelation(string uid, string productId, string locationId, string userId);
        public List<DepartmentModel> GetDepartmentByLocationId(string userId, string locationId);

        public bool UpdateDepartment(DepartmentModel model, string userId, string locationId);
    }
}
