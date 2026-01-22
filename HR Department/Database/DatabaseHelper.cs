using System;
using System.Data;
using System.Data.SqlClient;

namespace HR_Department
{
    public static class DatabaseHelper
    {
        //public static readonly string connectionString =
        //    "Data Source=DanteewPC\\EQS_TRINY56;Initial Catalog=KLIO_db;Integrated Security=True";

        public static string connectionString =
            "Data Source=(LocalDB)\\44;AttachDbFilename=|DataDirectory|\\KLIO_db.mdf;Integrated Security=True";


        public static DataTable GetStaff()
        {
            using var con = new SqlConnection(connectionString);
            using var da = new SqlDataAdapter("SELECT * FROM staff_table", con);

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public static void InsertStaff(Staff s)
        {
            Execute(@"
                INSERT INTO staff_table 
                (full_name, service, phone_number, email, birth_date, city, post_number)
                VALUES (@f,@s,@p,@e,@b,@c,@i)", s);
        }

        public static void UpdateStaff(Staff s)
        {
            Execute(@"
                UPDATE staff_table SET
                full_name=@f, service=@s, phone_number=@p,
                email=@e, birth_date=@b, city=@c, post_number=@i
                WHERE staff_id=@id", s, s.staff_id);
        }

        public static void DeleteStaff(int id)
        {
            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(
                "DELETE FROM staff_table WHERE staff_id=@id", con);

            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        private static void Execute(string sql, Staff s, int? id = null)
        {
            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@f", s.full_name);
            cmd.Parameters.AddWithValue("@s", s.service);
            cmd.Parameters.AddWithValue("@p", s.phone_number);
            cmd.Parameters.AddWithValue("@e", s.email);
            cmd.Parameters.AddWithValue("@b", s.birth_date);
            cmd.Parameters.AddWithValue("@c", s.city);
            cmd.Parameters.AddWithValue("@i", s.post_number);

            if (id.HasValue)
                cmd.Parameters.AddWithValue("@id", id.Value);

            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
