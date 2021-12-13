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
    }
}
