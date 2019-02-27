using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RideShareWebServices.Models
{
    public class UserProfileRatingModel
    {
        public int Id { get; set; }

        public String Student_EmployeeId { get; set; }

        public String UserName { get; set; }

        public String Phone { get; set; }

        public Nullable<int> PhoneStatus { get; set; }

        public Nullable<int> OneStar { get; set; }

        public Nullable<int> TwoStar { get; set; }

        public Nullable<int> ThreeStar { get; set; }

        public Nullable<int> FourStar { get; set; }

        public Nullable<int> FiveStar { get; set; }

    }
}