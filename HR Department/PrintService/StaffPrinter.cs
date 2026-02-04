using System;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace HR_Department
{
    public class StaffPrinter
    {
        private readonly ListView _listView;
        private readonly ColumnVisibilityManager _columnManager;

        public StaffPrinter(ListView listView, ColumnVisibilityManager columnManager)
        {
            _listView = listView;
            _columnManager = columnManager;
        }

        public void Print()
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;

                if (printDialog.ShowDialog() == true)
                {
                    FlowDocument document = CreatePrintDocument(printDialog.PrintableAreaWidth);
                    document.PageHeight = printDialog.PrintableAreaHeight;
                    document.PageWidth = printDialog.PrintableAreaWidth;
                    document.PagePadding = new Thickness(25);

                    FixedDocument fixedDoc = ConvertToFixedDocument(document,
                        printDialog.PrintableAreaWidth,
                        printDialog.PrintableAreaHeight);

                    printDialog.PrintDocument(fixedDoc.DocumentPaginator, "Список сотрудников");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка печати: {ex.Message}", ex);
            }
        }

        public void ShowPreview()
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;

                FlowDocument document = CreatePrintDocument(printDialog.PrintableAreaWidth);
                document.PageHeight = printDialog.PrintableAreaHeight;
                document.PageWidth = printDialog.PrintableAreaWidth;
                document.PagePadding = new Thickness(25);

                var previewWindow = new PrintPreviewWindow(document, this);
                previewWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка предварительного просмотра: {ex.Message}", ex);
            }
        }

        public void ShowQuickPreview()
        {
            try
            {
                // Используем стандартные размеры A4 в альбомной ориентации (ширина больше высоты)
                double pageWidth = 29.7 * 96; // A4 ширина в пикселях (альбомная ориентация)
                double pageHeight = 21 * 96;  // A4 высота в пикселях (альбомная ориентация)

                FlowDocument document = CreatePrintDocument(pageWidth);
                document.PageHeight = pageHeight;
                document.PageWidth = pageWidth;
                document.PagePadding = new Thickness(25);

                Window previewWindow = new Window
                {
                    Title = "Быстрый просмотр для печати",
                    Width = 1000,
                    Height = 700,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    WindowState = WindowState.Maximized
                };

                DocumentViewer viewer = new DocumentViewer
                {
                    Document = document,
                    Background = Brushes.White
                };

                // Добавляем панель инструментов
                DockPanel dockPanel = new DockPanel();

                StackPanel toolPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Height = 40,
                    Background = Brushes.LightGray,
                    Margin = new Thickness(0, 0, 0, 5)
                };

                Button closeButton = new Button
                {
                    Content = "Закрыть",
                    Width = 80,
                    Height = 30,
                    Margin = new Thickness(5)
                };

                closeButton.Click += (s, e) => previewWindow.Close();

                toolPanel.Children.Add(closeButton);

                dockPanel.Children.Add(toolPanel);
                DockPanel.SetDock(toolPanel, Dock.Top);
                dockPanel.Children.Add(viewer);

                previewWindow.Content = dockPanel;
                previewWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка быстрого просмотра: {ex.Message}", ex);
            }
        }

        private FlowDocument CreatePrintDocument(double printableAreaWidth)
        {
            var documentBuilder = new PrintDocumentBuilder(_listView, _columnManager);
            return documentBuilder.Build(printableAreaWidth);
        }

        private FixedDocument ConvertToFixedDocument(FlowDocument flowDoc, double pageWidth, double pageHeight)
        {
            FixedDocument fixedDoc = new FixedDocument();
            fixedDoc.DocumentPaginator.PageSize = new Size(pageWidth, pageHeight);

            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage
            {
                Width = pageWidth,
                Height = pageHeight
            };

            FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer
            {
                Document = flowDoc,
                Width = pageWidth - 50,
                Height = pageHeight - 50,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            };

            fixedPage.Children.Add(viewer);
            ((IAddChild)pageContent).AddChild(fixedPage);
            fixedDoc.Pages.Add(pageContent);

            return fixedDoc;
        }
    }
}