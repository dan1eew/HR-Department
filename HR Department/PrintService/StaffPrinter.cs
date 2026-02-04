using System;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace HR_Department
{
    public class StaffPrinter
    {
        private readonly ListView _listView;
        private readonly ColumnVisibilityManager _columnManager;

        public StaffPrinter(ListView listView)
        {
            _listView = listView;
            _columnManager = new ColumnVisibilityManager(listView);
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
                FlowDocument document = CreatePrintDocument(printDialog.PrintableAreaWidth);

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
                FlowDocument document = CreatePrintDocument(794); // A4 альбомная

                Window previewWindow = new Window
                {
                    Title = "Быстрый просмотр для печати",
                    Width = 800,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer
                {
                    Document = document
                };

                previewWindow.Content = viewer;
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