using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace HR_Department
{
    public partial class TableWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private readonly List<Staff> _staff = new();

        public TableWindow()
        {
            InitializeComponent();

            // Отображаем имя пользователя
            FullNameBox.Text = MainWindow.CurrentAdminFullName;

            // Таймер для времени
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (_, _) => DateTimeBox.Text = DateTime.Now.ToString("F");
            _timer.Start();

            // Подписка на Enter в поиске
            SearchText.KeyDown += SearchText_KeyDown;
        }

        // ========================= ЗАГРУЗКА ОКНА =========================
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            RefreshStaff("Данные успешно загружены");
        }

        // ========================= ДОБАВЛЕНИЕ =========================
        private void AddClick(object sender, RoutedEventArgs e)
        {
            var window = new StaffEditWindow();

            if (window.ShowDialog() == true)
                RefreshStaff("Сотрудник успешно добавлен");
        }

        // ========================= РЕДАКТИРОВАНИЕ =========================
        private void EditClick(object sender, RoutedEventArgs e)
        {
            if (ListViewStaff.SelectedItem is not StaffViewModel vm)
            {
                ShowWarning("Выберите сотрудника для редактирования");
                return;
            }

            var window = new StaffEditWindow(vm.GetStaff());

            if (window.ShowDialog() == true)
                RefreshStaff("Данные сотрудника обновлены");
        }

        // ========================= УДАЛЕНИЕ =========================
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (ListViewStaff.SelectedItem is not StaffViewModel vm)
            {
                ShowWarning("Выберите сотрудника для удаления");
                return;
            }

            var result = MessageBox.Show(
                $"Удалить сотрудника:\n{vm.full_name}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                DatabaseHelper.DeleteStaff(vm.staff_id);
                RefreshStaff("Сотрудник успешно удалён");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        // ========================= ОБНОВЛЕНИЕ =========================
        private void RefheshClick(object sender, RoutedEventArgs e)
        {
            RefreshStaff($"Данные обновлены вручную ({DateTime.Now:T})");
        }

        // ========================= ПОИСК (ENTER) =========================
        private void SearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ApplySearch();
        }

        private void ApplySearch()
        {
            string text = SearchText.Text?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(text))
            {
                ListViewStaff.ItemsSource = _staff.Select(s => new StaffViewModel(s)).ToList();
                ShowInfo("Поиск сброшен");
                return;
            }

            var filtered = _staff
                .Where(s =>
                    s.full_name.ToLower().Contains(text) ||
                    s.service.ToLower().Contains(text) ||
                    s.phone_number.Contains(text) ||
                    s.email.ToLower().Contains(text) ||
                    s.city.ToLower().Contains(text))
                .Select(s => new StaffViewModel(s))
                .ToList();

            ListViewStaff.ItemsSource = filtered;
            ShowInfo($"Найдено записей: {filtered.Count}");
        }

        // ========================= ЗАГРУЗКА ИЗ БД =========================
        private void RefreshStaff(string successMessage)
        {
            try
            {
                _staff.Clear();

                DataTable table = DatabaseHelper.GetStaff();
                foreach (DataRow row in table.Rows)
                    _staff.Add(Staff.FromDataRow(row));

                ListViewStaff.ItemsSource =
                    _staff.Select(s => new StaffViewModel(s)).ToList();

                ShowSuccess(successMessage);
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        // ========================= СТАТУС =========================
        private void ShowSuccess(string message)
        {
            StatusTableBox.Background = Brushes.Green;
            StatusTableBox.Text = message;
        }

        private void ShowError(string message)
        {
            StatusTableBox.Background = Brushes.Red;
            StatusTableBox.Text = message;
        }

        private void ShowWarning(string message)
        {
            StatusTableBox.Background = Brushes.DarkOrange;
            StatusTableBox.Text = message;
        }

        private void ShowInfo(string message)
        {
            StatusTableBox.Background = Brushes.Blue;
            StatusTableBox.Text = message;
        }

        // ========================= ВЫХОД =========================
        private void MainWindowClick(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }
}
