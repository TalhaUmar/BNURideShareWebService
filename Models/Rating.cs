using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RideShareWebServices.Models
{
    public class Rating
    {
        public int Id { get; set; }

        public Nullable<int> OneStar { get; set; }

        public Nullable<int> TwoStar { get; set; }

        public Nullable<int> ThreeStar { get; set; }

        public Nullable<int> FourStar { get; set; }

        public Nullable<int> FiveStar { get; set; }

        public int UserId { get; set; }
    }
}