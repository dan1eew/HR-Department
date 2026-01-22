using System;
using System.Data;
using System.Data.SqlClient;

namespace HR_Department
{
    public class DatabaseHelper
    {
        public static readonly string connectionString = "Data Source=DanteewPC\\EQS_TRINY56;Initial Catalog=KLIO_db;Integrated Security=True";

        // public static string connectionString = "Data Source=(LocalDB)\\44;AttachDbFilename=|DataDirectory|\\KLIO_db.mdf;Integrated Security=True";

        public static DataTable GetStaff()
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                const string query = "SELECT * FROM staff_table";
                using var command = new SqlCommand(query, connection);
                using var adapter = new SqlDataAdapter(command);

                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка подключения к базе данных: {ex.Message}", ex);
            }
        }
    }
}
