using RideShareWebServices.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RideShareWebServices.DAC
{
    public class RideDAC
    {
        private readonly string SELECT_All_RIDES = "Select * from Ride where AvailableSeats > 0 AND RideStatusId=@RideStatusId";
        private readonly string SELECT_All_RIDES_BY_USERID = "Select * from Ride where UserId=@UserId AND RideStatusId=@RideStatusId";
        private readonly string SELECT_BY_ID = "Select * from Ride where Id=@Id";

        public int Insert(Ride r)
        {
            SqlConnection con = DACHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("Ride_Insert", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DepartureTime", r.DepartureTime);
            cmd.Parameters.AddWithValue("@DepartureDate", r.DepartureDate);
            cmd.Parameters.AddWithValue("@PostDate", r.PostDate);
            cmd.Parameters.AddWithValue("@AvailableSeats", r.AvailableSeats);
            cmd.Parameters.AddWithValue("@CostPerKm", r.CostPerKm);
            cmd.Parameters.AddWithValue("@VehicleType", string.IsNullOrEmpty(r.VehicleType) ? Convert.DBNull : r.VehicleType);
            cmd.Parameters.AddWithValue("@RideFrequency", string.IsNullOrEmpty(r.RideFrequency) ? Convert.DBNull : r.RideFrequency);
            cmd.Parameters.AddWithValue("@Smoking", r.Smoking);
            cmd.Parameters.AddWithValue("@FoodDrinks", r.FoodDrinks);
            cmd.Parameters.AddWithValue("@Source", r.Source);
            cmd.Parameters.AddWithValue("@Destination", r.Destination);
            cmd.Parameters.AddWithValue("@CheckPoints", r.Checkpoints);
            cmd.Parameters.AddWithValue("@UserId", r.UserId);
            cmd.Parameters.AddWithValue("@RideStatusId", r.RideStatusId);
            con.Open();
            using (con)
            {
                int row = Convert.ToInt32(cmd.ExecuteScalar());
                return row;
            }
        }

        public List<Ride> SelectAllRides(Ride r)
        {
            SqlCommand cmd = new SqlCommand(SELECT_All_RIDES, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@RideStatusId", r.RideStatusId);
            return fetchRides(cmd);
        }

        public List<Ride> SelectAllRidesByUserId(Ride r)
        {
            SqlCommand cmd = new SqlCommand(SELECT_All_RIDES_BY_USERID, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@UserId", r.UserId);
            cmd.Parameters.AddWithValue("@RideStatusId", r.RideStatusId);
            return fetchRides(cmd);
        }

        public void RideUpdate(Ride r)
        {
            SqlConnection con = DACHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("Ride_Update", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", r.Id);
            cmd.Parameters.AddWithValue("@DepartureTime", r.DepartureTime);
            cmd.Parameters.AddWithValue("@DepartureDate", r.DepartureDate);
            cmd.Parameters.AddWithValue("@PostDate", r.PostDate);
            cmd.Parameters.AddWithValue("@AvailableSeats", r.AvailableSeats);
            cmd.Parameters.AddWithValue("@CostPerKm", r.CostPerKm);
            cmd.Parameters.AddWithValue("@VehicleType", string.IsNullOrEmpty(r.VehicleType) ? Convert.DBNull : r.VehicleType);
            cmd.Parameters.AddWithValue("@RideFrequency", string.IsNullOrEmpty(r.RideFrequency) ? Convert.DBNull : r.RideFrequency);
            cmd.Parameters.AddWithValue("@Smoking", r.Smoking);
            cmd.Parameters.AddWithValue("@FoodDrinks", r.FoodDrinks);
            cmd.Parameters.AddWithValue("@Source", r.Source);
            cmd.Parameters.AddWithValue("@Destination", r.Destination);
            cmd.Parameters.AddWithValue("@CheckPoints", r.Checkpoints);
            cmd.Parameters.AddWithValue("@UserId", r.UserId);
            cmd.Parameters.AddWithValue("@RideStatusId", r.RideStatusId);
            con.Open();
            using (con)
            {
                Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void RideStatusUpdate(Ride r)
        {
            SqlConnection con = DACHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("RideStatus_Update", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", r.Id);
            cmd.Parameters.AddWithValue("@RideStatusId", r.RideStatusId);
            con.Open();
            using (con)
            {
                Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public Ride SelectById(int id)
        {
            SqlCommand cmd = new SqlCommand(SELECT_BY_ID, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@Id", id);
            List<Ride> temp = fetchRides(cmd);
            return (temp != null) ? temp[0] : null;
        }

        public void DeleteRide(int id)
        {
            SqlCommand cmd = new SqlCommand("Ride_Delete", DACHelper.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            SqlConnection con = cmd.Connection;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        private List<Ride> fetchRides(SqlCommand cmd)
        {
            SqlConnection con = cmd.Connection;
            List<Ride> rlist = null;
            con.Open();
            using (con)
            {
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    rlist = new List<Ride>();
                    while (dr.Read())
                    {
                        Ride r = new Ride();
                        r.Id = Convert.ToInt32(dr["Id"]);
                        r.DepartureTime = Convert.ToString(dr["DepartureTime"]);
                        r.DepartureDate = Convert.ToString(dr["DepartureDate"]);
                        r.PostDate = Convert.ToString(dr["PostDate"]);
                        r.AvailableSeats = Convert.ToInt32(dr["AvailableSeats"]);
                        r.CostPerKm = Convert.ToInt32(dr["CostPerKm"]);
                        r.VehicleType = Convert.ToString(dr["VehicleType"]);
                        r.RideFrequency = Convert.ToString(dr["RideFrequency"]);
                        r.Smoking = Convert.ToInt32(dr["Smoking"]);
                        r.FoodDrinks = Convert.ToInt32(dr["FoodDrinks"]);
                        r.Source = Convert.ToString(dr["Source"]);
                        r.Destination = Convert.ToString(dr["Destination"]);
                        r.Checkpoints = Convert.ToString(dr["Checkpoints"]);
                        r.UserId = Convert.ToInt32(dr["UserId"]);
                        r.RideStatusId = Convert.ToInt32(dr["RideStatusId"]);
                        rlist.Add(r);
                    }
                    rlist.TrimExcess();
                }
            }
            return rlist;
        }
    }
}