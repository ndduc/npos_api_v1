using POS_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface ILocationLogic
    {
        public bool AddLocation(LocationModel model, string userId);
        public IEnumerable<LocationModel> GetLocationByUserId(string userId);
        public bool VerifyUIdExist(string uid);
    }
}
