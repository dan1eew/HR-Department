using System.Windows.Controls;

namespace HR_Department
{
    public class TableDataExtractor
    {
        public string GetCellText(StaffViewModel vm, GridViewColumn column)
        {
            if (vm == null) return "";

            try
            {
                string columnHeader = column.Header as string;

                return columnHeader switch
                {
                    "ID" => vm.staff_id.ToString(),
                    "ФИО" => FormatText(vm.full_name, 50),
                    "Отдел" => FormatText(vm.service, 50),
                    "Телефон" => FormatPhoneNumber(vm.phone_number),
                    "Email" => FormatText(vm.email, 50),
                    "Дата рождения" => vm.birth_date_str ?? "",
                    "Город" => FormatText(vm.city, 50),
                    "Индекс" => vm.post_number.ToString(),
                    _ => GetCellTextByBinding(vm, column)
                };
            }
            catch
            {
                return "";
            }
        }

        private string FormatText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return "";

            if (text.Length > maxLength)
            {
                return text.Substring(0, maxLength - 3) + "...";
            }

            return text;
        }

        private string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return "";

            phone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            if (phone.Length == 11 && phone.StartsWith("8"))
            {
                return $"+7 ({phone.Substring(1, 3)}) {phone.Substring(4, 3)}-{phone.Substring(7, 2)}-{phone.Substring(9, 2)}";
            }
            else if (phone.Length == 10)
            {
                return $"+7 ({phone.Substring(0, 3)}) {phone.Substring(3, 3)}-{phone.Substring(6, 2)}-{phone.Substring(8, 2)}";
            }

            return phone;
        }

        private string GetCellTextByBinding(object item, GridViewColumn column)
        {
            try
            {
                if (column.DisplayMemberBinding is System.Windows.Data.Binding binding)
                {
                    string propertyName = binding.Path.Path;
                    var property = item.GetType().GetProperty(propertyName);
                    if (property != null)
                    {
                        var value = property.GetValue(item);
                        return value?.ToString() ?? "";
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
    }
}