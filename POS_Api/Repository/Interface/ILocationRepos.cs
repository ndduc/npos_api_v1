using POS_Api.Model;
using System.Collections.Generic;

namespace POS_Api.Repository.Interface
{
    public interface ILocationRepos
    {
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);
        public bool AddLocationExecution(LocationModel model);
        public IEnumerable<LocationModel> GetLocationByUserIdExecution(string userId);
    }
}
