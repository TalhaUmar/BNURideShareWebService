using RideShareWebServices.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RideShareWebServices.DAC
{
    public class RideRequestDAC
    {
        private readonly string SELECT_RIDE_REQUEST_BY_RIDEID = "Select * from RideRequest where RideId=@RideId";
        private readonly string SELECT_RIDE_REQUEST_BY_PASSENGERID = "Select * from RideRequest where PassengerId=@PassengerId";
        private readonly string SELECT_ALL_PENDING_RIDE_REQUEST_BY_RIDEID = "Select * from RideRequest where RideId=@RideId AND Status=@Status";
        private readonly string SELECT_All_ACCEPTED_REQUEST_BY_PASSENGERID = "Select * from RideRequest where PassengerId=@PassengerId AND Status=@Status";
        private readonly string SELECT_All_ACCEPTED_REQUEST_BY_RIDEID = "Select * from RideRequest where RideId=@RideId AND Status=@Status";
        private readonly string SELECT_EXISTING_REQUEST_BY_PASSENGERID_AND_RIDEID = "Select * from RideRequest where RideId=@RideId AND PassengerId=@PassengerId";

        public int Insert(RideRequest r)
        {
            SqlConnection con = DACHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("RideRequest_Insert", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RideId", r.RideId);
            cmd.Parameters.AddWithValue("@PassengerId", r.PassengerId);
            cmd.Parameters.AddWithValue("@Status", r.RequestStatus);
            cmd.Parameters.AddWithValue("@PickupLocation", r.PickupLocation);
            cmd.Parameters.AddWithValue("@Destination", r.Destination);
            cmd.Parameters.AddWithValue("@TotalAmount", r.TotalAmount);
            con.Open();
            using (con)
            {
                int row = Convert.ToInt32(cmd.ExecuteScalar());
                return row;
            }
        }

        public void RideRequestStatusUpdate(RideRequest r)
        {
            SqlConnection con = DACHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("RideRequest_Status_Update", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RideId", r.RideId);
            cmd.Parameters.AddWithValue("@Status", r.RequestStatus);
            con.Open();
            using (con)
            {
                Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public RideRequest SelectByRideId(int id)
        {
            SqlCommand cmd = new SqlCommand(SELECT_RIDE_REQUEST_BY_RIDEID, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@RideId", id);
            List<RideRequest> temp = fetchRideRequest(cmd);
            return (temp != null) ? temp[0] : null;
        }

        public List<RideRequest> SelectAllPendingRideRequestByRideId(int rid)
        {
            SqlCommand cmd = new SqlCommand(SELECT_ALL_PENDING_RIDE_REQUEST_BY_RIDEID, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@RideId", rid);
            cmd.Parameters.AddWithValue("@Status", 1);
            List<RideRequest> rlist = fetchRideRequest(cmd);
            return rlist;
        }

        public RideRequest SelectRequestByRideIdAndPassengerId(int pid, int rid)
        {
            SqlCommand cmd = new SqlCommand(SELECT_EXISTING_REQUEST_BY_PASSENGERID_AND_RIDEID, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@RideId", rid);
            cmd.Parameters.AddWithValue("@PassengerId", pid);
            List<RideRequest> temp = fetchRideRequest(cmd);
            return (temp != null) ? temp[0] : null;
        }

        public RideRequest SelectByPassengerId(int id)
        {
            SqlCommand cmd = new SqlCommand(SELECT_RIDE_REQUEST_BY_PASSENGERID, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@PassengerId", id);
            List<RideRequest> temp = fetchRideRequest(cmd);
            return (temp != null) ? temp[0] : null;
        }

        public List<RideRequest> SelectALLAcceptedRequestByPassengerId(RideRequest rr)
        {
            SqlCommand cmd = new SqlCommand(SELECT_All_ACCEPTED_REQUEST_BY_PASSENGERID, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@PassengerId", rr.PassengerId);
            cmd.Parameters.AddWithValue("@Status", rr.RequestStatus);
            return fetchRideRequest(cmd);
        }

        public List<RideRequest> SelectALLAcceptedRequestByRideId(RideRequest rr)
        {
            SqlCommand cmd = new SqlCommand(SELECT_All_ACCEPTED_REQUEST_BY_RIDEID, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@RideId", rr.RideId);
            cmd.Parameters.AddWithValue("@Status", rr.RequestStatus);
            return fetchRideRequest(cmd);
        }

        public void DeleteRideRequest(RideRequest rr)
        {
            SqlCommand cmd = new SqlCommand("RideRequest_Delete", DACHelper.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PassengerId", rr.PassengerId);
            cmd.Parameters.AddWithValue("@RideId", rr.RideId);
            cmd.Parameters.AddWithValue("@Status", rr.RequestStatus);
            SqlConnection con = cmd.Connection;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void DeleteAllRideRequestsByRideId(int rid)
        {
            SqlCommand cmd = new SqlCommand("Delete_AllRequests_ByRideId", DACHelper.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RideId", rid);
            SqlConnection con = cmd.Connection;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        private List<RideRequest> fetchRideRequest(SqlCommand cmd)
        {
            SqlConnection con = cmd.Connection;
            List<RideRequest> rlist = null;
            con.Open();
            using (con)
            {
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    rlist = new List<RideRequest>();
                    while (dr.Read())
                    {
                        RideRequest r = new RideRequest();
                        r.Id = Convert.ToInt32(dr["Id"]);
                        r.RideId = Convert.ToInt32(dr["RideId"]);
                        r.PassengerId = Convert.ToInt32(dr["PassengerId"]);
                        r.RequestStatus = Convert.ToString(dr["Status"]);
                        r.PickupLocation = Convert.ToString(dr["PickupLocation"]);
                        r.Destination = Convert.ToString(dr["Destination"]);
                        if (Convert.IsDBNull(dr["TotalAmount"]))
                        {
                            r.TotalAmount = null;
                        }
                        else
                        {
                            r.TotalAmount = Convert.ToInt32(dr["TotalAmount"]);
                        }
                        
                        rlist.Add(r);
                    }
                    rlist.TrimExcess();
                }
            }
            return rlist;
        }
    }
}