using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Core.Interface
{
    public interface ILocationUserRelationLogic
    {
        public bool AddRelationLocationUser(string muserId, string userId, string locationId, string reason);

        public bool IsRelationLocationUserExist(string userId, string locationId, string reason);
    }
}
