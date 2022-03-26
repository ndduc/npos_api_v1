using System.Collections.Generic;

namespace POS_Api.Model
{
    public class UserLocationModel : UserModel
    {
         IEnumerable<LocationModel> UserLocations { get; set; }
    }
}
