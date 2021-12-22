using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Repository.Interface
{
    public interface IDepartmentRepos
    {
        public bool UpdateDepartmentExecution(DepartmentModel model);
        public List<DepartmentModel> GetDepartmentByLocationIdExecution(string locationId);
        public bool AddDepartmentExecution(DepartmentModel model);
        public bool AddDepartmentProductRelationExecution(string uid, string productId, string locationId, string userId);
        public bool VerifyDepartmentProductRelationExist(string uid, string productId, string locationId);
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);

        public bool AddDepartmentExecutionFromList(List<string> deptIdlist, string productId, string locationId, string userId);


        public int GetDepartmentPaginateCount(string locId);
        public IEnumerable<DepartmentModel> GetDepartmentPaginateByDefault(string locId, int startIdx, int endIdx);

        public DepartmentModel GetDepartmentById(string locId, string departmentId);

        public IEnumerable<DepartmentModel> GetDepartmentByDescription(string locId, string description);
    }
}
