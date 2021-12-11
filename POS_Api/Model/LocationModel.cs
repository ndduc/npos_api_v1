using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Model
{
    public class LocationModel
    {
        public string UId{ get; set; }
        public string Name { get; set;}
        public string Address { get; set; }
        public int ZipCode { get; set; }
        public string State { get; set; }
        public string PhoneNumber { get; set; }
        public string AddedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }

        public bool IsError { get; set; }
        public string Error { get; set; }

        public string RelationReason { get; set; }

        public LocationModel()
        {

        }

        public LocationModel(string uid, string Name, string Address,
            int ZipCode, string State, string PhoneNumber, string AddedDateTime, string UpdatedDateTimem,
            string RelationReason)
        {
            this.UId = uid;
            this.Name = Name;
            this.Address = Address;
            this.State = State;
            this.ZipCode = ZipCode;
            this.PhoneNumber = PhoneNumber;
            this.AddedDateTime = AddedDateTime;
            this.UpdatedDateTime = UpdatedDateTime;
            this.RelationReason = RelationReason;
        }
    }
}
