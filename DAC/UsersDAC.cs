using RideShareWebServices.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RideShareWebServices.DAC
{
    public class UsersDAC
    {
        private readonly string SELECT_ALL_USERS = "Select * from Users";
        private readonly string SELECT_BY_Std_EMPID = "Select * from Users where Std_EmpId=@Student_EmployeeId";
        private readonly string SELECT_BY_Phone = "Select * from Users where Phone=@Phone";
        private readonly string SELECT_BY_STD_EMPID_AND_PASSWORD = "Select * from Users where Std_EmpId=@Student_EmployeeId AND Password=@Password";
        private readonly string SELECT_BY_ID = "Select * from Users where Id=@Id";

        public int Insert(Users u)
        {
            SqlConnection con = DACHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("User_Insert", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Std_EmpId", u.Student_EmployeeId);
            cmd.Parameters.AddWithValue("@UserName", string.IsNullOrEmpty(u.UserName) ? Convert.DBNull : u.UserName);
            cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(u.Phone) ? Convert.DBNull : u.Phone);
            cmd.Parameters.AddWithValue("@PhoneStatus", u.PhoneStatus);
            cmd.Parameters.AddWithValue("@Password", string.IsNullOrEmpty(u.Password) ? Convert.DBNull : u.Password);
            con.Open();
            using (con)
            {
                int row = Convert.ToInt32(cmd.ExecuteScalar());
                return row;
            }
        }

        public void UserUpdate(Users u)
        {
            SqlConnection con = DACHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("User_Update", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", u.Id);
            cmd.Parameters.AddWithValue("@Std_EmpId", u.Student_EmployeeId);
            cmd.Parameters.AddWithValue("@UserName", string.IsNullOrEmpty(u.UserName) ? Convert.DBNull : u.UserName);
            cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(u.Phone) ? Convert.DBNull : u.Phone);
            cmd.Parameters.AddWithValue("@PhoneStatus", u.PhoneStatus);
            cmd.Parameters.AddWithValue("@Password", string.IsNullOrEmpty(u.Password) ? Convert.DBNull : u.Password);
            cmd.Parameters.AddWithValue("@Token", string.IsNullOrEmpty(u.Token) ? Convert.DBNull : u.Token);
            con.Open();
            using (con)
            {
                Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void UpdatePhone(Users u)
        {
            SqlConnection con = DACHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("User_Phone_Update", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Std_EmpId", u.Student_EmployeeId);
            cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(u.Phone) ? Convert.DBNull : u.Phone);
            cmd.Parameters.AddWithValue("@PhoneStatus", u.PhoneStatus);
            con.Open();
            using (con)
            {
                Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void UpdatePassword(Users u)
        {
            SqlConnection con = DACHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("User_Password_Update", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Std_EmpId", u.Student_EmployeeId);
            cmd.Parameters.AddWithValue("@Password", string.IsNullOrEmpty(u.Password) ? Convert.DBNull : u.Password);
            con.Open();
            using (con)
            {
                Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public Users SelectById(int id)
        {
            SqlCommand cmd = new SqlCommand(SELECT_BY_ID, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@Id", id);
            List<Users> temp = fetchUsers(cmd);
            return (temp != null) ? temp[0] : null;
        }

        public Users SelectByPhone(string phone)
        {
            SqlCommand cmd = new SqlCommand(SELECT_BY_Phone, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@Phone", phone);
            List<Users> temp = fetchUsers(cmd);
            return (temp != null) ? temp[0] : null;
        }

        public Users SelectByStdEmpId(string id)
        {
            SqlCommand cmd = new SqlCommand(SELECT_BY_Std_EMPID, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@Student_EmployeeId", id);
            List<Users> temp = fetchUsers(cmd);
            return (temp != null) ? temp[0] : null;
        }

        public Users SelectByStdEmpIdAndPassword(string std_empId, string password)
        {
            SqlCommand cmd = new SqlCommand(SELECT_BY_STD_EMPID_AND_PASSWORD, DACHelper.GetConnection());
            cmd.Parameters.AddWithValue("@Student_EmployeeId", std_empId);
            cmd.Parameters.AddWithValue("@Password", password);
            List<Users> temp = fetchUsers(cmd);
            return (temp != null) ? temp[0] : null;
        }

        public List<Users> SelectAllUsers()
        {
            SqlCommand cmd = new SqlCommand(SELECT_ALL_USERS, DACHelper.GetConnection());
            return fetchUsers(cmd);

        }

        public void DeleteUser(int id)
        {
            SqlCommand cmd = new SqlCommand("User_Delete", DACHelper.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            SqlConnection con = cmd.Connection;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        private List<Users> fetchUsers(SqlCommand cmd)
        {
            SqlConnection con = cmd.Connection;
            List<Users> ulist = null;
            con.Open();
            using (con)
            {
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    ulist = new List<Users>();
                    while (dr.Read())
                    {
                        Users u = new Users();
                        u.Id = Convert.ToInt32(dr["Id"]);
                        u.Student_EmployeeId = Convert.ToString(dr["Std_EmpId"]);
                        u.UserName = Convert.ToString(dr["UserName"]);
                        u.Phone = Convert.ToString(dr["Phone"]);
                        if (Convert.IsDBNull(dr["PhoneStatus"]))
                        {
                            u.PhoneStatus = null;
                        }
                        else
                        {
                            u.PhoneStatus = Convert.ToInt32(dr["PhoneStatus"]);
                        }
                        u.Password = Convert.ToString(dr["Password"]);
                        u.Token = Convert.ToString(dr["Token"]);
                        ulist.Add(u);
                    }
                    ulist.TrimExcess();
                }
            }
            return ulist;
        }
    }
}