using System;
using System.Data;

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

    public static Staff FromDataRow(DataRow row)
    {
        try
        {
            return new Staff
            {
                staff_id = GetValue<int>(row, "staff_id"),
                full_name = GetValue<string>(row, "full_name"),
                service = GetValue<string>(row, "service"),
                phone_number = GetValue<string>(row, "phone_number"),
                email = GetValue<string>(row, "email"),
                birth_date = GetValue<DateTime>(row, "birth_date"),
                city = GetValue<string>(row, "city"),
                post_number = GetValue<int>(row, "post_number"),
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка создания сотрудника: {ex.Message}");
            return new Staff();
        }
    }
    private static T GetValue<T>(DataRow row, string column)
    {
        return row[column] != DBNull.Value ? (T)Convert.ChangeType(row[column], typeof(T)) : default;
    }

    private static string GetValue(DataRow row, string column, string defaultValue)
    {
        return row[column] != DBNull.Value ? row[column].ToString() : defaultValue;
    }
}
