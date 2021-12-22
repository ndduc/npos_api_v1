using POS_Api.Model;
using System.Collections.Generic;

namespace POS_Api.Core.Interface
{
    public interface IDepartmentLogic
    {
        public bool AddDepartment(DepartmentModel model, string userId, string locationId);
        public bool AddDepartmentProductRelation(string uid, string productId, string locationId, string userId);
        public List<DepartmentModel> GetDepartmentByLocationId(string userId, string locationId);

        public bool UpdateDepartment(DepartmentModel model, string userId, string locationId);

        public int GetDepartmentPaginateCount(Dictionary<string, string> param);
        public IEnumerable<DepartmentModel> GetDepartmentPaginate(Dictionary<string, string> param);

        public DepartmentModel GetDepartmentById(string userId, string locId, string departmentId);

        public IEnumerable<DepartmentModel> GetDepartmentByDescription(string userId, string locId, string description);
    }
}
