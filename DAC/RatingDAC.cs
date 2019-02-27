using RideShareWebServices.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RideShareWebServices.DAC
{
    public class RatingDAC
    {
        private readonly string SELECT_BY_USERID = "Select * from Rating where UserId=@UserId";

        public int Insert(Rating r)
        {
            SqlConnection con = DACHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("Rating_Insert", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OneStar", r.OneStar.HasValue ? r.OneStar : 0);
            cmd.Parameters.AddWithValue("@TwoStar", r.TwoStar.HasValue ? r.TwoStar : 0);
            cmd.Parameters.AddWithValue("@ThreeStar", r.ThreeStar.HasValue ? r.ThreeStar : 0);
            cmd.Parameters.AddWithValue("@FourStar", r.FourStar.HasValue ? r.FourStar : 0);
            cmd.Parameters.AddWithValue("@FiveStar", r.FiveStar.HasValue ? r.FiveStar : 0);
            cmd.Parameters.AddWithValue("@UserId", r.UserId);
            con.Open();
            using (con)
            {
                int row = Convert.ToInt32(cmd.ExecuteScalar());
                return row;
            }
        }

        public void RatingUpdate(Rating r)
        {
            SqlConnection con = DACHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("Rating_Update", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OneStar", r.OneStar);
            cmd.Parameters.AddWithValue("@TwoStar", r.TwoStar);
            cmd.Parameters.AddWithValue("@ThreeStar", r.ThreeStar);
            cmd.Parameters.AddWithValue("@FourStar", r.FourStar);
            cmd.Parameters.AddWithValue("@FiveStar", r.FiveStar);
            cmd.Parameters.AddWithValue("@UserId", r.UserId);
            con.Open();
            using (con)
            {
                Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public Rating SelectByUserId(int id)
        {
            SqlCommand cmd = new SqlCommand(SELECT_BY_USERID, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@UserId", id);
            List<Rating> temp = fetchRating(cmd);
            return (temp != null) ? temp[0] : null;
        }

        private List<Rating> fetchRating(SqlCommand cmd)
        {
            SqlConnection con = cmd.Connection;
            List<Rating> rlist = null;
            con.Open();
            using (con)
            {
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    rlist = new List<Rating>();
                    while (dr.Read())
                    {
                        Rating r = new Rating();
                        r.Id = Convert.ToInt32(dr["Id"]);
                        if (Convert.IsDBNull(dr["OneStar"]))
                        {
                            r.OneStar = null;
                        }
                        else
                        {
                            r.OneStar = Convert.ToInt32(dr["OneStar"]);
                        }
                        if (Convert.IsDBNull(dr["TwoStar"]))
                        {
                            r.TwoStar = null;
                        }
                        else
                        {
                            r.TwoStar = Convert.ToInt32(dr["TwoStar"]);
                        }
                        if (Convert.IsDBNull(dr["ThreeStar"]))
                        {
                            r.ThreeStar = null;
                        }
                        else
                        {
                            r.ThreeStar = Convert.ToInt32(dr["ThreeStar"]);
                        }
                        if (Convert.IsDBNull(dr["FourStar"]))
                        {
                            r.FourStar = null;
                        }
                        else
                        {
                            r.FourStar = Convert.ToInt32(dr["FourStar"]);
                        }
                        if (Convert.IsDBNull(dr["FiveStar"]))
                        {
                            r.FiveStar = null;
                        }
                        else
                        {
                            r.FiveStar = Convert.ToInt32(dr["FiveStar"]);
                        }
                        r.UserId = Convert.ToInt32(dr["UserId"]);
                        rlist.Add(r);
                    }
                    rlist.TrimExcess();
                }
            }
            return rlist;
        }
    }
}