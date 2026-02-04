using System;
using System.Windows;
using System.Windows.Documents;

namespace HR_Department
{
    public partial class PrintPreviewWindow : Window
    {
        private readonly FlowDocument _document;
        private readonly StaffPrinter _printer;

        public PrintPreviewWindow(FlowDocument document, StaffPrinter printer)
        {
            InitializeComponent();
            _document = document;
            _printer = printer;

            DocumentViewer.Document = _document;

            PrintButton.Click += PrintButton_Click;
            CloseButton.Click += (s, e) => Close();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _printer.Print();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}