using System;
using System.Data;
using System.Data.SqlClient;

namespace Lab4
{
    public class AdoAssistant
    {
        private string connectionString;

        public AdoAssistant(string cs)
        {
            connectionString = cs;
        }

        public DataTable GetAll()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlDataAdapter da =
                    new SqlDataAdapter("SELECT * FROM Clients", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        public void Insert(string name, string phone, string address, decimal amount)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"INSERT INTO Clients (Name, Phone, Address, OrderAmount)
                               VALUES (@Name, @Phone, @Address, @Amount)";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@Amount", amount);

                cmd.ExecuteNonQuery();
            }
        }

        public void Update(int id, string name, string phone, string address, decimal amount)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"UPDATE Clients
                               SET Name=@Name, Phone=@Phone, Address=@Address, OrderAmount=@Amount
                               WHERE ID=@ID";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@Amount", amount);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd =
                    new SqlCommand("DELETE FROM Clients WHERE ID=@ID", conn);

                cmd.Parameters.AddWithValue("@ID", id);

                cmd.ExecuteNonQuery();
            }
        }
        public bool PhoneExists(string phone)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd =
                    new SqlCommand("SELECT COUNT(*) FROM Clients WHERE Phone=@Phone", conn);

                cmd.Parameters.AddWithValue("@Phone", phone);

                return (int)cmd.ExecuteScalar() > 0;
            }
        }
        public int GetIdByPhone(string phone)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd =
                    new SqlCommand("SELECT ID FROM Clients WHERE Phone=@Phone", conn);

                cmd.Parameters.AddWithValue("@Phone", phone);

                object result = cmd.ExecuteScalar();

                return result != null ? (int)result : -1;
            }
        }
    }
}