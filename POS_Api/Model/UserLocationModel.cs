using System.Collections.Generic;

namespace POS_Api.Model
{
    public class UserLocationModel : UserModel
    {
         public IList<LocationModel> UserLocations { get; set; }
         public IEnumerable<string> LocationIds { get; set; }
    }
}
