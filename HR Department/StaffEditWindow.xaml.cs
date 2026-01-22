using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace HR_Department
{
    public partial class StaffEditWindow : Window
    {
        private readonly bool _isEdit;
        private readonly Staff _staff;

        public StaffEditWindow()
        {
            InitializeComponent();
            _staff = new Staff();
        }

        public StaffEditWindow(Staff staff) : this()
        {
            _staff = staff;
            _isEdit = true;
            TitleLabel.FontSize = 22;
            TitleLabel.Content = "Редактирование сотрудника";
            Fill();
        }

        private void Fill()
        {
            FullNameBox.Text = _staff.full_name;
            ServiceBox.Text = _staff.service;
            PhoneBox.Text = _staff.phone_number;
            EmailBox.Text = _staff.email;
            BirthDateBox.Text = _staff.birth_date.ToString("dd.MM.yyyy");
            CityBox.Text = _staff.city;
            PostNumberBox.Text = _staff.post_number.ToString();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;

            _staff.full_name = FullNameBox.Text.Trim();
            _staff.service = ServiceBox.Text.Trim();
            _staff.phone_number = PhoneBox.Text.Trim();
            _staff.email = EmailBox.Text.Trim();
            _staff.birth_date = DateTime.ParseExact(
                BirthDateBox.Text.Trim(),
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture);
            _staff.city = CityBox.Text.Trim();
            _staff.post_number = int.Parse(PostNumberBox.Text);

            try
            {
                if (_isEdit)
                    DatabaseHelper.UpdateStaff(_staff);
                else
                    DatabaseHelper.InsertStaff(_staff);

                DialogResult = true;
            }
            catch (Exception ex)
            {
                StatusBox.Text = ex.Message;
            }
        }

        private bool Validate()
        {
            if (FullNameBox.Text.Length == 0 || FullNameBox.Text.Length > 150)
                return Error("ФИО обязательно (до 150 символов)");

            if (ServiceBox.Text.Length == 0 || ServiceBox.Text.Length > 100)
                return Error("Отдел обязателен (до 100)");

            if (!Regex.IsMatch(PhoneBox.Text, @"^(\+7|8)\d{10}$"))
                return Error("Телефон: +7XXXXXXXXXX или 8XXXXXXXXXX");

            if (EmailBox.Text.Length > 150 ||
                !Regex.IsMatch(EmailBox.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return Error("Некорректный Email");

            if (!DateTime.TryParseExact(
                    BirthDateBox.Text.Trim(),
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime birth))
                return Error("Дата должна быть в формате dd.MM.yyyy");

            if (birth >= DateTime.Today)
                return Error("Дата рождения должна быть меньше текущей");

            if (CityBox.Text.Length == 0 || CityBox.Text.Length > 100)
                return Error("Город обязателен (до 100)");

            if (!int.TryParse(PostNumberBox.Text, out _))
                return Error("Индекс должен быть числом");

            StatusBox.Text = "";
            return true;
        }

        private bool Error(string text)
        {
            StatusBox.Foreground = Brushes.Red;
            StatusBox.Text = text;
            return false;
        }

        private void CancelClick(object sender, RoutedEventArgs e) => Close();
    }
}
