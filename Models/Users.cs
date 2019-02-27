using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RideShareWebServices.Models
{
    public class Users
    {
        public int Id { get; set; }

        public String Student_EmployeeId { get; set; }

        public String UserName { get; set; }

        public String Phone { get; set; }

        public Nullable<int> PhoneStatus { get; set; }

        public String Password { get; set; }

        public String Token { get; set; }
    }
}