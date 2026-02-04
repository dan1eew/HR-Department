using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HR_Department
{
    public partial class SettingsWindow : Window
    {
        // Словарь для хранения видимости столбцов
        private readonly Dictionary<string, bool> _columnVisibility;
        private readonly Dictionary<string, CheckBox> _checkBoxes = new();

        // Свойство для получения текущих настроек
        public Dictionary<string, bool> CurrentVisibility => new Dictionary<string, bool>(_columnVisibility);

        // Конструктор с передачей текущих настроек
        public SettingsWindow(Dictionary<string, bool> currentVisibility)
        {
            InitializeComponent();

            // Копируем текущие настройки или используем значения по умолчанию
            _columnVisibility = currentVisibility ?? GetDefaultVisibility();

            InitializeCheckBoxes();
            UpdateSelectAllButtonState();
        }

        // Конструктор по умолчанию (для обратной совместимости)
        public SettingsWindow() : this(null)
        {
        }

        /// <summary>
        /// Получение настроек по умолчанию
        /// </summary>
        private Dictionary<string, bool> GetDefaultVisibility()
        {
            return new Dictionary<string, bool>
            {
                { "ID", true },
                { "ФИО", true },
                { "Отдел", true },
                { "Телефон", true },
                { "Email", true },
                { "Дата рождения", true },
                { "Город", true },
                { "Индекс", true }
            };
        }

        /// <summary>
        /// Инициализация CheckBox для каждого столбца
        /// </summary>
        private void InitializeCheckBoxes()
        {
            ColumnsPanel.Children.Clear();
            _checkBoxes.Clear();

            // Порядок отображения столбцов (можно настраивать)
            var columnsOrder = new List<string>
            {
                "ID", "ФИО", "Отдел", "Телефон",
                "Email", "Дата рождения", "Город", "Индекс"
            };

            foreach (var columnName in columnsOrder)
            {
                if (!_columnVisibility.ContainsKey(columnName))
                    continue;

                // Создаем контейнер для чекбокса с описанием
                var container = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(5, 8, 5, 8)
                };

                var checkBox = new CheckBox
                {
                    Content = GetColumnDisplayName(columnName),
                    IsChecked = _columnVisibility[columnName],
                    FontSize = 14,
                    FontWeight = FontWeights.Normal,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Tag = columnName,
                    ToolTip = GetColumnTooltip(columnName)
                };

                checkBox.Checked += CheckBox_Changed;
                checkBox.Unchecked += CheckBox_Changed;

                // Добавляем иконку или индикатор
                var icon = new TextBlock
                {
                    Text = GetColumnIcon(columnName),
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI Symbol"),
                    FontSize = 16,
                    Margin = new Thickness(0, 0, 8, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };

                container.Children.Add(icon);
                container.Children.Add(checkBox);

                _checkBoxes[columnName] = checkBox;
                ColumnsPanel.Children.Add(container);
            }

            // Добавляем счетчик выбранных колонок
            UpdateSelectedCount();
        }

        /// <summary>
        /// Получение отображаемого имени столбца
        /// </summary>
        private string GetColumnDisplayName(string columnName)
        {
            return columnName switch
            {
                "ID" => "ID сотрудника",
                "ФИО" => "Фамилия Имя Отчество",
                "Отдел" => "Подразделение/Отдел",
                "Телефон" => "Номер телефона",
                "Email" => "Электронная почта",
                "Дата рождения" => "Дата рождения",
                "Город" => "Город проживания",
                "Индекс" => "Почтовый индекс",
                _ => columnName
            };
        }

        /// <summary>
        /// Получение иконки для столбца
        /// </summary>
        private string GetColumnIcon(string columnName)
        {
            return columnName switch
            {
                "ID" => "🔢",
                "ФИО" => "👤",
                "Отдел" => "🏢",
                "Телефон" => "📱",
                "Email" => "📧",
                "Дата рождения" => "🎂",
                "Город" => "🏙️",
                "Индекс" => "📮",
                _ => "📋"
            };
        }

        /// <summary>
        /// Получение подсказки для столбца
        /// </summary>
        private string GetColumnTooltip(string columnName)
        {
            return columnName switch
            {
                "ID" => "Уникальный идентификатор сотрудника",
                "ФИО" => "Полное имя сотрудника",
                "Отдел" => "Подразделение или отдел, в котором работает сотрудник",
                "Телефон" => "Контактный номер телефона",
                "Email" => "Адрес электронной почты",
                "Дата рождения" => "Дата рождения сотрудника",
                "Город" => "Город проживания сотрудника",
                "Индекс" => "Почтовый индекс адреса проживания",
                _ => "Информация о сотруднике"
            };
        }

        /// <summary>
        /// Обработка изменения состояния чекбокса
        /// </summary>
        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            UpdateSelectedCount();
            UpdateSelectAllButtonState();
        }

        /// <summary>
        /// Обновление счетчика выбранных колонок
        /// </summary>
        private void UpdateSelectedCount()
        {
            int selectedCount = _checkBoxes.Count(kvp => kvp.Value.IsChecked == true);
            int totalCount = _checkBoxes.Count;

            // Можно отображать счетчик в заголовке или отдельном элементе
            // Для простоты пока просто обновляем состояние кнопок
        }

        /// <summary>
        /// Обновление состояния кнопки "Выбрать все"
        /// </summary>
        private void UpdateSelectAllButtonState()
        {
            int selectedCount = _checkBoxes.Count(kvp => kvp.Value.IsChecked == true);
            int totalCount = _checkBoxes.Count;

            if (selectedCount == totalCount)
            {
                BtnSelectAll.Content = "Снять все";
                BtnSelectAll.ToolTip = "Снять выделение со всех столбцов";
            }
            else
            {
                BtnSelectAll.Content = "Выбрать все";
                BtnSelectAll.ToolTip = "Выбрать все столбцы для отображения";
            }
        }

        /// <summary>
        /// Выбрать все столбцы / Снять все
        /// </summary>
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            int selectedCount = _checkBoxes.Count(kvp => kvp.Value.IsChecked == true);
            int totalCount = _checkBoxes.Count;

            bool newState = selectedCount != totalCount; // Если не все выбраны - выбираем все

            foreach (var checkBox in _checkBoxes.Values)
            {
                checkBox.IsChecked = newState;
            }

            UpdateSelectAllButtonState();
            UpdateSelectedCount();
        }

        /// <summary>
        /// Снять выделение со всех столбцов
        /// </summary>
        private void DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var checkBox in _checkBoxes.Values)
            {
                checkBox.IsChecked = false;
            }

            UpdateSelectAllButtonState();
            UpdateSelectedCount();
        }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Обновляем словарь видимости
            foreach (var kvp in _checkBoxes)
            {
                _columnVisibility[kvp.Key] = kvp.Value.IsChecked == true;
            }

            // Проверяем, что выбрана хотя бы одна колонка
            int visibleCount = _columnVisibility.Count(kvp => kvp.Value);
            if (visibleCount == 0)
            {
                MessageBox.Show("Для отображения таблицы должна быть видна хотя бы одна колонка!\n\n" +
                               "Пожалуйста, выберите один или несколько столбцов.",
                               "Внимание",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Отменить изменения
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}