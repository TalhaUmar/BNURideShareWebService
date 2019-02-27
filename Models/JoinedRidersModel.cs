using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RideShareWebServices.Models
{
    public class JoinedRidersModel
    {
        public int Id { get; set; }

        public String UserName { get; set; }

        public int RideId { get; set; }

        public int PassengerId { get; set; }

        public String PickupLocation { get; set; }

        public String Destination { get; set; }

        public String RequestStatus { get; set; }

        public Nullable<int> TotalAmount { get; set; }
    }
}