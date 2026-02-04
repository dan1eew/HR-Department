using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ModuleApp.Printing
{
    /// <summary>
    /// Класс для печати DataGrid с учетом видимости столбцов
    /// </summary>
    public class DataGridPrinter
    {
        /// <summary>
        /// Печатает DataGrid с учетом только видимых столбцов
        /// </summary>
        /// <param name="dataGrid">DataGrid для печати</param>
        /// <param name="title">Заголовок документа</param>
        /// <param name="showPreview">Показывать предварительный просмотр</param>
        public static void PrintDataGrid(DataGrid dataGrid, string title = "Печать данных", bool showPreview = true)
        {
            try
            {
                // Создаем документ для печати
                FlowDocument document = CreatePrintDocument(dataGrid, title);

                if (showPreview)
                {
                    // Показываем предварительный просмотр
                    ShowPrintPreview(document, title);
                }
                else
                {
                    // Печатаем напрямую
                    PrintDirectly(document, title);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати: {ex.Message}", "Ошибка печати",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Создает документ для печати
        /// </summary>
        private static FlowDocument CreatePrintDocument(DataGrid dataGrid, string title)
        {
            FlowDocument document = new FlowDocument
            {
                PageHeight = 29.7 * 96, // A4 высота в пикселях (96 DPI)
                PageWidth = 21 * 96,    // A4 ширина в пикселях
                PagePadding = new Thickness(50, 40, 50, 40),
                FontFamily = new FontFamily("Times New Roman"),
                FontSize = 10
            };

            // Заголовок документа
            if (!string.IsNullOrEmpty(title))
            {
                Paragraph header = new Paragraph(new Run(title))
                {
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                document.Blocks.Add(header);
            }

            // Информация о дате печати
            Paragraph dateInfo = new Paragraph(new Run($"Дата печати: {DateTime.Now:dd.MM.yyyy HH:mm}"))
            {
                FontSize = 9,
                FontStyle = FontStyles.Italic,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 0, 0, 10)
            };
            document.Blocks.Add(dateInfo);

            // Создаем таблицу для данных
            Table table = CreateDataTable(dataGrid);
            document.Blocks.Add(table);

            // Информация о количестве строк
            Paragraph footer = new Paragraph(new Run($"Всего записей: {dataGrid.Items.Count}"))
            {
                FontSize = 9,
                FontStyle = FontStyles.Italic,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(0, 20, 0, 0)
            };
            document.Blocks.Add(footer);

            return document;
        }

        /// <summary>
        /// Создает таблицу с данными из DataGrid
        /// </summary>
        private static Table CreateDataTable(DataGrid dataGrid)
        {
            Table table = new Table();

            // Получаем только видимые столбцы
            var visibleColumns = dataGrid.Columns
                .Where(col => col.Visibility == Visibility.Visible)
                .ToList();

            // Создаем колонки таблицы
            for (int i = 0; i < visibleColumns.Count; i++)
            {
                table.Columns.Add(new TableColumn());
            }

            // Настраиваем стили таблицы
            table.CellSpacing = 0;
            table.Background = Brushes.White;

            // Создаем группу строк для заголовков
            TableRowGroup headerGroup = new TableRowGroup();

            // Заголовки столбцов
            TableRow headerRow = new TableRow
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240))
            };

            foreach (var column in visibleColumns)
            {
                TableCell headerCell = new TableCell(new Paragraph(new Run(GetColumnHeader(column))))
                {
                    FontWeight = FontWeights.Bold,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    Padding = new Thickness(5, 3, 5, 3),
                    TextAlignment = GetTextAlignment(column)
                };

                headerRow.Cells.Add(headerCell);
            }

            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);

            // Создаем группу строк для данных
            TableRowGroup dataGroup = new TableRowGroup();

            // Добавляем данные
            foreach (var item in dataGrid.Items)
            {
                TableRow dataRow = new TableRow();

                foreach (var column in visibleColumns)
                {
                    string cellValue = GetCellValue(column, item);

                    TableCell dataCell = new TableCell(new Paragraph(new Run(cellValue)))
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1, 0, 1, 1),
                        Padding = new Thickness(5, 3, 5, 3),
                        TextAlignment = GetTextAlignment(column)
                    };

                    // Альтернируем цвет строк для лучшей читаемости
                    if (dataGroup.Rows.Count % 2 == 1)
                    {
                        dataCell.Background = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));
                    }

                    dataRow.Cells.Add(dataCell);
                }

                dataGroup.Rows.Add(dataRow);
            }

            table.RowGroups.Add(dataGroup);

            return table;
        }

        /// <summary>
        /// Получает значение ячейки в зависимости от типа колонки
        /// </summary>
        private static string GetCellValue(DataGridColumn column, object item)
        {
            try
            {
                if (item == null) return string.Empty;

                // Обработка DataGridTextColumn
                if (column is DataGridTextColumn textColumn)
                {
                    if (textColumn.Binding is Binding binding)
                    {
                        string propertyPath = binding.Path.Path;
                        return GetPropertyValue(item, propertyPath);
                    }
                }
                // Обработка DataGridTemplateColumn
                else if (column is DataGridTemplateColumn templateColumn)
                {
                    // Для TemplateColumn сложнее получить значение
                    // Можно попробовать найти TextBlock в шаблоне или использовать другие подходы
                    return "[Шаблон]";
                }
                // Обработка DataGridCheckBoxColumn
                else if (column is DataGridCheckBoxColumn checkBoxColumn)
                {
                    if (checkBoxColumn.Binding is Binding binding)
                    {
                        string propertyPath = binding.Path.Path;
                        string value = GetPropertyValue(item, propertyPath);

                        if (bool.TryParse(value, out bool boolValue))
                        {
                            return boolValue ? "Да" : "Нет";
                        }

                        return value;
                    }
                }
                // Обработка DataGridComboBoxColumn
                else if (column is DataGridComboBoxColumn comboBoxColumn)
                {
                    if (comboBoxColumn.SelectedValueBinding is Binding binding)
                    {
                        string propertyPath = binding.Path.Path;
                        return GetPropertyValue(item, propertyPath);
                    }
                }
            }
            catch
            {
                // В случае ошибки возвращаем пустую строку
            }

            return string.Empty;
        }

        /// <summary>
        /// Получает значение свойства объекта по пути
        /// </summary>
        private static string GetPropertyValue(object item, string propertyPath)
        {
            if (string.IsNullOrEmpty(propertyPath) || item == null)
                return string.Empty;

            try
            {
                // Разделяем путь на свойства (на случай вложенных свойств)
                string[] properties = propertyPath.Split('.');
                object currentObject = item;

                foreach (string property in properties)
                {
                    if (currentObject == null) return string.Empty;

                    Type type = currentObject.GetType();
                    PropertyInfo propertyInfo = type.GetProperty(property);

                    if (propertyInfo == null) return string.Empty;

                    currentObject = propertyInfo.GetValue(currentObject, null);
                }

                return FormatValue(currentObject);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Форматирует значение для отображения
        /// </summary>
        private static string FormatValue(object value)
        {
            if (value == null) return string.Empty;

            try
            {
                // Форматирование в зависимости от типа
                return value switch
                {
                    DateTime dateTime => dateTime.ToString("dd.MM.yyyy"),
                    decimal decimalValue => decimalValue.ToString("N2"),
                    double doubleValue => doubleValue.ToString("N2"),
                    float floatValue => floatValue.ToString("N2"),
                    bool boolValue => boolValue ? "Да" : "Нет",
                    _ => value.ToString()
                };
            }
            catch
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// Получает заголовок колонки
        /// </summary>
        private static string GetColumnHeader(DataGridColumn column)
        {
            if (column.Header == null) return string.Empty;

            return column.Header switch
            {
                string str => str,
                TextBlock textBlock => textBlock.Text,
                CheckBox checkBox => checkBox.Content?.ToString() ?? string.Empty,
                _ => column.Header.ToString()
            };
        }

        /// <summary>
        /// Определяет выравнивание текста на основе типа данных колонки
        /// </summary>
        private static TextAlignment GetTextAlignment(DataGridColumn column)
        {
            // По умолчанию выравнивание по левому краю
            TextAlignment alignment = TextAlignment.Left;

            // Если колонка содержит числовые данные, выравниваем по правому краю
            if (column is DataGridTextColumn textColumn)
            {
                if (textColumn.Binding is Binding binding)
                {
                    string propertyPath = binding.Path.Path.ToLower();

                    // Проверяем, содержит ли название свойства числовые типы
                    if (propertyPath.Contains("id") ||
                        propertyPath.Contains("count") ||
                        propertyPath.Contains("quantity") ||
                        propertyPath.Contains("amount") ||
                        propertyPath.Contains("sum") ||
                        propertyPath.Contains("price") ||
                        propertyPath.Contains("total"))
                    {
                        alignment = TextAlignment.Right;
                    }
                }
            }

            return alignment;
        }

        /// <summary>
        /// Показывает предварительный просмотр печати
        /// </summary>
        private static void ShowPrintPreview(FlowDocument document, string title)
        {
            // Создаем окно предварительного просмотра
            Window previewWindow = new Window
            {
                Title = $"Предварительный просмотр: {title}",
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowState = WindowState.Maximized
            };

            // Создаем DocumentViewer для просмотра документа
            DocumentViewer documentViewer = new DocumentViewer
            {
                Document = document,
                Background = Brushes.White
            };

            // Добавляем панель инструментов
            DockPanel dockPanel = new DockPanel();

            // Панель инструментов
            StackPanel toolPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Height = 40,
                Background = Brushes.LightGray,
                Margin = new Thickness(0, 0, 0, 5)
            };

            // Кнопка печати
            Button printButton = new Button
            {
                Content = "Печать",
                Width = 80,
                Height = 30,
                Margin = new Thickness(5)
            };

            printButton.Click += (s, e) => PrintDirectly(document, title);

            // Кнопка закрытия
            Button closeButton = new Button
            {
                Content = "Закрыть",
                Width = 80,
                Height = 30,
                Margin = new Thickness(5)
            };

            closeButton.Click += (s, e) => previewWindow.Close();

            toolPanel.Children.Add(printButton);
            toolPanel.Children.Add(closeButton);

            dockPanel.Children.Add(toolPanel);
            DockPanel.SetDock(toolPanel, Dock.Top);
            dockPanel.Children.Add(documentViewer);

            previewWindow.Content = dockPanel;
            previewWindow.ShowDialog();
        }

        /// <summary>
        /// Печатает документ напрямую
        /// </summary>
        private static void PrintDirectly(FlowDocument document, string title)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                // Настраиваем размеры документа под выбранный принтер
                document.PageHeight = printDialog.PrintableAreaHeight;
                document.PageWidth = printDialog.PrintableAreaWidth;

                // Печатаем документ
                printDialog.PrintDocument(
                    ((IDocumentPaginatorSource)document).DocumentPaginator,
                    title);

                MessageBox.Show("Документ отправлен на печать", "Печать",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Экспортирует DataGrid в PDF (дополнительная функция)
        /// </summary>
        public static void ExportToPdf(DataGrid dataGrid, string filePath, string title = "Экспорт данных")
        {
            try
            {
                // Для экспорта в PDF потребуется дополнительная библиотека (например, iTextSharp или PdfSharp)
                MessageBox.Show("Экспорт в PDF требует установки дополнительных библиотек.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Реализация экспорта в PDF будет зависеть от выбранной библиотеки
                // Вот пример с использованием iTextSharp (нужно установить пакет iTextSharp):
                /*
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 40f, 40f, 40f, 40f);
                    PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    
                    // Добавление заголовка
                    if (!string.IsNullOrEmpty(title))
                    {
                        Paragraph pdfTitle = new Paragraph(title, 
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14));
                        pdfTitle.Alignment = Element.ALIGN_CENTER;
                        pdfDoc.Add(pdfTitle);
                        pdfDoc.Add(new Paragraph("\n"));
                    }
                    
                    // Создание таблицы
                    var visibleColumns = dataGrid.Columns
                        .Where(col => col.Visibility == Visibility.Visible)
                        .ToList();
                    
                    PdfPTable pdfTable = new PdfPTable(visibleColumns.Count);
                    
                    // Заголовки таблицы
                    foreach (var column in visibleColumns)
                    {
                        PdfPCell headerCell = new PdfPCell(new Phrase(GetColumnHeader(column)));
                        headerCell.BackgroundColor = new BaseColor(240, 240, 240);
                        pdfTable.AddCell(headerCell);
                    }
                    
                    // Данные таблицы
                    foreach (var item in dataGrid.Items)
                    {
                        foreach (var column in visibleColumns)
                        {
                            string cellValue = GetCellValue(column, item);
                            pdfTable.AddCell(new PdfPCell(new Phrase(cellValue)));
                        }
                    }
                    
                    pdfDoc.Add(pdfTable);
                    pdfDoc.Close();
                }
                
                MessageBox.Show($"Документ сохранен: {filePath}", "Экспорт в PDF",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                */
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в PDF: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}