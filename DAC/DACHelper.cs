using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RideShareWebServices.DAC
{
    internal class DACHelper
    {
        internal static SqlConnection GetConnection() 
        {
            return new SqlConnection(@"workstation id=bnurideshare.mssql.somee.com;packet size=4096;user id=talha;pwd=12345678;data source=bnurideshare.mssql.somee.com;persist security info=False;initial catalog=bnurideshare");
        }
    }
}