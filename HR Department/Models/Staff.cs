using System;
using System.Data;
using System.Text.RegularExpressions;

namespace HR_Department
{
    public class Staff
    {
        public int staff_id { get; set; }
        public string full_name { get; set; }
        public string service { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public DateTime birth_date { get; set; }
        public string city { get; set; }
        public int post_number { get; set; }

        public string birth_date_str => birth_date.ToString("dd.MM.yyyy");

        /// <summary>Валидация под БД</summary>
        public static bool Validate(Staff s,out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(s.full_name) || s.full_name.Length > 150)
                error = "Некорректное ФИО";

            else if (string.IsNullOrWhiteSpace(s.service) || s.service.Length > 100)
                error = "Некорректный отдел";

            else if (string.IsNullOrWhiteSpace(s.phone_number) || s.phone_number.Length > 20)
                error = "Некорректный телефон";

            else if (!Regex.IsMatch(s.email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                error = "Некорректный Email";

            else if (s.birth_date > DateTime.Now)
                error = "Дата рождения не может быть в будущем";

            else if (string.IsNullOrWhiteSpace(s.city) || s.city.Length > 100)
                error = "Некорректный город";

            else if (s.post_number <= 0)
                error = "Некорректный индекс";

            return error == string.Empty;
        }

        public static Staff FromDataRow(DataRow row) => new()
        {
            staff_id = Convert.ToInt32(row["staff_id"]),
            full_name = row["full_name"].ToString(),
            service = row["service"].ToString(),
            phone_number = row["phone_number"].ToString(),
            email = row["email"].ToString(),
            birth_date = Convert.ToDateTime(row["birth_date"]),
            city = row["city"].ToString(),
            post_number = Convert.ToInt32(row["post_number"])
        };
    }
}
