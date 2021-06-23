using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;


namespace WINSTLD
{
    public class SQLCON
    {
        //string ConnectionString = "Data Source=DB-OPS1-TEST\\MDS;Initial Catalog=DB-OPS1-TEST\\MDS;User ID=sa;Password=P@ssw0rd";
        string ConnectionString = "Data Source=192.168.1.220\\SQLDB;Initial Catalog=STLD;User ID=stld;Password=P@ssw0rd";


        SqlConnection con;


        private static string connnstring= "Data Source=192.168.1.220\\SQLDB;Initial Catalog = STLD; User ID = stld; Password=P@ssw0rd";

        private static string connnstring3 = "Data Source=192.168.1.220\\SQLDB;Initial Catalog = Npepm; User ID = stld; Password=P@ssw0rd";

        private static string connnstring4 = "Data Source=192.168.1.208\\TKTSRVVM01;Initial Catalog = Npepm; User ID = stld; Password=P@ssw0rd";

        public static string ConnectionString2
        {
            get
            {
                return connnstring;
            }
            set
            {
                ConnectionString2 = value;
            }
        }

        public static string ConnectionString3
        {
            get
            {
                return connnstring3;
            }
            set
            {
                ConnectionString3 = value;
            }
        }
        public void OpenConnection()
        {
            con = new SqlConnection(ConnectionString);
            con.Open();
            

        }
        public void CloseConnection()
        {
            con = new SqlConnection(ConnectionString);
            con.Close();

        }

        
    }
   
}
