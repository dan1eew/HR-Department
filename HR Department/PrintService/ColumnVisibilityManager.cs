using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace HR_Department
{
    public class ColumnVisibilityManager
    {
        private readonly ListView _listView;
        private readonly Dictionary<string, bool> _columnVisibility;

        public ColumnVisibilityManager(ListView listView)
        {
            _listView = listView;
            _columnVisibility = InitializeDefaultVisibility();
        }

        private Dictionary<string, bool> InitializeDefaultVisibility()
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

        public void ShowSettingsDialog()
        {
            var settingsWindow = new SettingsWindow(_columnVisibility);

            if (settingsWindow.ShowDialog() == true)
            {
                foreach (var kvp in settingsWindow.CurrentVisibility)
                {
                    if (_columnVisibility.ContainsKey(kvp.Key))
                    {
                        _columnVisibility[kvp.Key] = kvp.Value;
                    }
                }

                ApplyVisibility();
            }
        }

        public void ApplyVisibility()
        {
            if (_listView.View is GridView gridView)
            {
                foreach (GridViewColumn column in gridView.Columns)
                {
                    string columnName = GetColumnName(column);
                    if (columnName != null && _columnVisibility.ContainsKey(columnName))
                    {
                        column.Width = _columnVisibility[columnName] ? GetDefaultWidth(columnName) : 0;
                    }
                }
            }
        }

        private string GetColumnName(GridViewColumn column)
        {
            return column.Header?.ToString() switch
            {
                "ID" => "ID",
                "ФИО" => "ФИО",
                "Отдел" => "Отдел",
                "Телефон" => "Телефон",
                "Email" => "Email",
                "Дата рождения" => "Дата рождения",
                "Город" => "Город",
                "Индекс" => "Индекс",
                _ => null
            };
        }

        private double GetDefaultWidth(string columnName)
        {
            return columnName switch
            {
                "ID" => 50,
                "ФИО" => 120,
                "Отдел" => 120,
                "Телефон" => 100,
                "Email" => 120,
                "Дата рождения" => 80,
                "Город" => 120,
                "Индекс" => 70,
                _ => 100
            };
        }

        public IEnumerable<GridViewColumn> GetVisibleColumns()
        {
            if (_listView.View is GridView gridView)
            {
                return gridView.Columns
                    .Cast<GridViewColumn>()
                    .Where(c => IsColumnVisible(c))
                    .ToList();
            }
            return Enumerable.Empty<GridViewColumn>();
        }

        private bool IsColumnVisible(GridViewColumn column)
        {
            string columnName = GetColumnName(column);
            return columnName != null &&
                   _columnVisibility.ContainsKey(columnName) &&
                   _columnVisibility[columnName];
        }
    }
}