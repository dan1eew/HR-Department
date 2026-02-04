using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace HR_Department
{
    public class PrintDocumentBuilder
    {
        private readonly ListView _listView;
        private readonly ColumnVisibilityManager _columnManager;

        public PrintDocumentBuilder(ListView listView, ColumnVisibilityManager columnManager)
        {
            _listView = listView;
            _columnManager = columnManager;
        }

        public FlowDocument Build(double printableAreaWidth)
        {
            FlowDocument document = new FlowDocument
            {
                FontFamily = new FontFamily("Times New Roman"),
                FontSize = 10,
                PagePadding = new Thickness(25)
            };

            AddHeader(document);
            AddDate(document);
            AddTable(document, printableAreaWidth);
            AddFooter(document);

            return document;
        }

        private void AddHeader(FlowDocument document)
        {
            Paragraph header = new Paragraph(new Run("Список сотрудников"))
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 15)
            };
            document.Blocks.Add(header);
        }

        private void AddDate(FlowDocument document)
        {
            Paragraph dateParagraph = new Paragraph(new Run($"Дата печати: {DateTime.Now:dd.MM.yyyy HH:mm}"))
            {
                FontSize = 10,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(0, 0, 0, 15)
            };
            document.Blocks.Add(dateParagraph);
        }

        private void AddTable(FlowDocument document, double printableAreaWidth)
        {
            var visibleColumns = _columnManager.GetVisibleColumns().ToList();
            var table = CreateTable(visibleColumns, printableAreaWidth);

            AddTableHeaders(table, visibleColumns);
            AddTableData(table, visibleColumns);
            CalculateColumnWidths(table, visibleColumns.Count, printableAreaWidth - 50);

            document.Blocks.Add(table);
        }

        private Table CreateTable(IEnumerable<GridViewColumn> columns, double printableAreaWidth)
        {
            Table table = new Table
            {
                CellSpacing = 0,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0.5)
            };

            int columnCount = columns.Count();
            for (int i = 0; i < columnCount; i++)
            {
                table.Columns.Add(new TableColumn());
            }

            return table;
        }

        private void AddTableHeaders(Table table, List<GridViewColumn> columns)
        {
            TableRowGroup headerRowGroup = new TableRowGroup();
            TableRow headerRow = new TableRow();
            headerRowGroup.Rows.Add(headerRow);

            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                string headerText = column.Header as string ?? "Без названия";

                TableCell cell = new TableCell(new Paragraph(new Run(headerText)))
                {
                    FontWeight = FontWeights.Bold,
                    Background = Brushes.LightGray,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0.5),
                    Padding = new Thickness(4),
                    TextAlignment = TextAlignment.Center
                };

                headerRow.Cells.Add(cell);
            }

            table.RowGroups.Add(headerRowGroup);
        }

        private void AddTableData(Table table, List<GridViewColumn> columns)
        {
            TableRowGroup dataRowGroup = new TableRowGroup();

            if (_listView.ItemsSource != null)
            {
                var dataExtractor = new TableDataExtractor();

                foreach (var item in _listView.Items)
                {
                    if (item is StaffViewModel vm)
                    {
                        TableRow row = new TableRow();

                        for (int i = 0; i < columns.Count; i++)
                        {
                            var column = columns[i];
                            string cellText = dataExtractor.GetCellText(vm, column);

                            TableCell cell = new TableCell(new Paragraph(new Run(cellText)))
                            {
                                BorderBrush = Brushes.Black,
                                BorderThickness = new Thickness(0.5),
                                Padding = new Thickness(4),
                                TextAlignment = GetTextAlignment(column.Header as string)
                            };

                            row.Cells.Add(cell);
                        }

                        dataRowGroup.Rows.Add(row);
                    }
                }
            }

            table.RowGroups.Add(dataRowGroup);
        }

        private void CalculateColumnWidths(Table table, int columnCount, double availableWidth)
        {
            if (columnCount == 0) return;

            double baseWidth = availableWidth / columnCount;
            double[] columnWidths = new double[columnCount];
            double totalWidth = 0;

            for (int i = 0; i < columnCount; i++)
            {
                // Настраиваем ширину в зависимости от типа колонки
                columnWidths[i] = i switch
                {
                    0 => baseWidth * 0.5,  // ID
                    3 => baseWidth * 0.8,  // Телефон
                    _ => baseWidth
                };

                totalWidth += columnWidths[i];
            }

            double scaleFactor = availableWidth / totalWidth;

            for (int i = 0; i < columnCount; i++)
            {
                double width = Math.Max(40, columnWidths[i] * scaleFactor);
                table.Columns[i].Width = new GridLength(width);
            }
        }

        private TextAlignment GetTextAlignment(string columnHeader)
        {
            return columnHeader switch
            {
                "ID" => TextAlignment.Center,
                "Телефон" => TextAlignment.Center,
                "Дата рождения" => TextAlignment.Center,
                "Индекс" => TextAlignment.Center,
                _ => TextAlignment.Left
            };
        }

        private void AddFooter(FlowDocument document)
        {
            if (_listView.Items.Count > 0)
            {
                Paragraph summary = new Paragraph(new Run($"Всего записей: {_listView.Items.Count}"))
                {
                    FontSize = 10,
                    FontStyle = FontStyles.Italic,
                    Margin = new Thickness(0, 15, 0, 0)
                };
                document.Blocks.Add(summary);
            }
        }
    }
}