using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace HR_Department
{
    public partial class TableWindow : Window
    {
        private DispatcherTimer _timer;
        private readonly List<Staff> _staff = new();
        public TableWindow()
        {
            InitializeComponent();
            FullNameBox.Text = MainWindow.CurrentUserFullName;
            InitializeTimer();
        }
        private void MainWindowClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            mainwindow.Show();
            this.Close();
        }
        /// <summary>Загружает данные при запуске окна</summary>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadStaff(out string errorMessage);
            }
            catch (Exception ex)
            {
                StatusTableBox.Background = Brushes.Red;
                StatusTableBox.Text = $"Ошибка при загрузке {ex.Message}";
            }
        }

        /// <summary>Загружает сотрудников из базы данных</summary>
        private bool LoadStaff(out string errorMessage)
        {
            errorMessage = null;

            try
            {
                _staff.Clear();
                var dataTable = DatabaseHelper.GetStaff();

                foreach (DataRow row in dataTable.Rows)
                    _staff.Add(Staff.FromDataRow(row));

                var viewModels = new List<StaffViewModel>();
                foreach (var staff in _staff)
                    viewModels.Add(new StaffViewModel(staff));

                ListViewStaff.ItemsSource = viewModels;

                return true; // успех
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                StatusTableBox.Background = Brushes.Red; 
                StatusTableBox.Text = $"Ошибка при загрузки данных {ex.Message}";
                return false; // ошибка
            }
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => DateTimeBox.Text = DateTime.Now.ToString("F");
            _timer.Start();
        }

        private void RefheshClick(object sender, RoutedEventArgs e)
        {
            StatusTableBox.FontSize = 18;

            if (LoadStaff(out string error))
            {
                StatusTableBox.Background = Brushes.Green;
                StatusTableBox.Text =  $"Данные успешно обновлены. Время обновления: {DateTime.Now}";
            }
            else
            {
                StatusTableBox.Background = Brushes.Red;
                StatusTableBox.Text = $"Ошибка при загрузке данных: {error}";
            }
        }
    }
}
