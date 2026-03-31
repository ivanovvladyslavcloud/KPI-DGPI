using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Lab4
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            connectionString =
                ConfigurationManager
                .ConnectionStrings["connectionString_ADO"]
                .ConnectionString;
        }

        public DataTable GetClients()
        {
            DataTable table = new DataTable();

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Clients";

                SqlDataAdapter adapter =
                    new SqlDataAdapter(query, connection);

                adapter.Fill(table);
            }

            return table;
        }
    }
}