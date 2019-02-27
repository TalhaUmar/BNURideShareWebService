using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RideShareWebServices.Models
{
    public class RideModel
    {
        public int Id { get; set; }

        public String DepartureTime { get; set; }

        public String DepartureDate { get; set; }

        public String PostDate { get; set; }

        public int AvailableSeats { get; set; }

        public int CostPerKm { get; set; }

        public String VehicleType { get; set; }

        public String RideFrequency { get; set; }

        public int Smoking { get; set; }

        public int FoodDrinks { get; set; }

        public String Source { get; set; }

        public String Destination { get; set; }

        public String Checkpoints { get; set; }

        public int UserId { get; set; }

        public int RideStatusId { get; set; }

    }
}