using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Repository.Interface
{
    public  interface ILocationRepos
    {
        public bool VerifyUIdUnique(string uid);
        public bool VerifyUIdExist(string uid);
        public bool AddLocationExecution(LocationModel model);

        public IEnumerable<LocationModel> GetLocationByUserIdExecution(string userId);
    }
}
