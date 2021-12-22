using POS_Api.Model;
using System.Collections.Generic;

namespace POS_Api.Core.Interface
{
    public interface ILocationLogic
    {
        public bool AddLocation(LocationModel model, string userId);
        public IEnumerable<LocationModel> GetLocationByUserId(string userId);
    }
}
