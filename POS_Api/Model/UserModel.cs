using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Model
{
    public class UserModel
    {
        public string UId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string AddedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }

        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }

        public string UserType { get; set; }

        public bool IsAuthorize { get; set; }
        public bool IsError { get; set; }
        public string Error { get; set; }
        public UserModel() { }
        public UserModel(string UId, string UserName, string Password, string FirstName, string LastName, string Email, string Email2,
            string Phone, string Address, string UserType ,string AddedDateTime, string UpdatedDateTime)
        {
            this.UId = UId;
            this.UserName = UserName;
            this.Password = Password;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.Email2 = Email2;
            this.Phone = Phone;
            this.Address = Address;
            this.UserType = UserType;
            this.AddedDateTime = AddedDateTime;
            this.UpdatedDateTime = UpdatedDateTime;
        }
    }
}
