using FrontEnd.Controller;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace FrontEnd.Reports
{
    public class ReportViewer : Control
    {
        static ReportViewer() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ReportViewer), new FrameworkPropertyMetadata(typeof(ReportViewer)));
        
        public ReportViewer() => PrintCommand = new CMD(PrintFixDocs);

        #region FileName
        public static readonly DependencyProperty FileNameProperty =
         DependencyProperty.Register(nameof(FileName), typeof(string), typeof(ReportViewer), new PropertyMetadata());
        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }
        #endregion

        #region SelectedPage
        public static readonly DependencyProperty SelectedPageProperty =
         DependencyProperty.Register(nameof(SelectedPage), typeof(ReportPage), typeof(ReportViewer), new PropertyMetadata());
        public ReportPage SelectedPage
        {
            get => (ReportPage)GetValue(SelectedPageProperty);
            set => SetValue(SelectedPageProperty, value);
        }
        #endregion

        #region ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty =
         DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<ReportPage>), typeof(ReportViewer), new PropertyMetadata());
        public IEnumerable<ReportPage> ItemsSource
        {
            get => (IEnumerable<ReportPage>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        #endregion

        #region PrintCommand
        public static readonly DependencyProperty PrintCommandProperty =
         DependencyProperty.Register(nameof(PrintCommand), typeof(ICommand), typeof(ReportViewer), new PropertyMetadata());
        public ICommand PrintCommand
        {
            get => (ICommand)GetValue(PrintCommandProperty);
            set => SetValue(PrintCommandProperty, value);
        }
        #endregion

        private void PrintFixDocs()
        {
            LocalPrintServer printServer = new();
            PrintQueueCollection printQueues = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
            PrintQueue? pdfPrinter = printQueues.FirstOrDefault(pq => pq.Name.Contains("PDF")) ?? throw new Exception("No PDF Printer was found.");
            PrintDialog printDialog = new()
            {
                PrintQueue = pdfPrinter
            };


            FixedDocument doc = new();
            ReportPage first_page = ItemsSource.First();
            doc.DocumentPaginator.PageSize = new Size(first_page.PageWidth, first_page.PageHeight);

            foreach (ReportPage page in ItemsSource)
            {
                PageContent pageContent = new();
                FixedPage fixedPage = new ()
                {
                    Width = page.PageWidth,
                    Height = page.PageHeight
                };

                page.Measure(new Size(fixedPage.Width, fixedPage.Height));
                page.Arrange(new Rect(new Point(), fixedPage.DesiredSize));
                page.UpdateLayout();

                FixedPage.SetLeft(page, 0);
                FixedPage.SetTop(page, 0);
                fixedPage.Children.Add(page.Copy());

                ((IAddChild)pageContent).AddChild(fixedPage);

                doc.Pages.Add(pageContent);                
            }
            
            printDialog.PrintDocument(doc.DocumentPaginator, "Printing");
        }

    }
}
