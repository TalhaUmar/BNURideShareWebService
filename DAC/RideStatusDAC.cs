using RideShareWebServices.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RideShareWebServices.DAC
{
    public class RideStatusDAC
    {
        private readonly string SELECT_BY_ID = "Select * from RideStatus where Id=@Id";

        public RideStatus SELECTById(int id)
        {
            SqlCommand cmd = new SqlCommand(SELECT_BY_ID, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@Id", id);
            List<RideStatus> temp = fetchRidetatus(cmd);
            return (temp != null) ? temp[0] : null;
        }

        private List<RideStatus> fetchRidetatus(SqlCommand cmd)
        {
            SqlConnection con = cmd.Connection;
            List<RideStatus> rlist = null;
            con.Open();
            using (con)
            {
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    rlist = new List<RideStatus>();
                    while (dr.Read())
                    {
                        RideStatus r = new RideStatus();
                        r.Id = Convert.ToInt32(dr["Id"]);
                        r.Status = Convert.ToString(dr["Status"]);
                        rlist.Add(r);
                    }
                    rlist.TrimExcess();
                }
            }
            return rlist;
        }
    }
}